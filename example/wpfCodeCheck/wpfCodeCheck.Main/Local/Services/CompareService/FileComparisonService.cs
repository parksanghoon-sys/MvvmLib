using wpfCodeCheck.Domain.Models;
using wpfCodeCheck.Domain.Enums;
using System.IO;
using wpfCodeCheck.Main.Local.Services.CompareService;

namespace wpfCodeCheck.Main.Local.Servies
{
    
    /// <summary>
    /// 파일 비교 전용 서비스
    /// 디렉토리 탐색은 다른 서비스에서 처리하고, 이미 탐색된 파일 리스트들을 비교
    /// </summary>
    public class FileComparisonService : ICompareService
    {
        /// <summary>
        /// 두 파일 리스트를 비교하여 차이점을 찾음
        /// </summary>
        public async Task<List<FileTreeModel>> CompareFileTreesAsync(IList<FileTreeModel> inputFiles, IList<FileTreeModel> outputFiles)
        {
            var differences = new List<FileTreeModel>();

            await Task.Run(() =>
            {
                // 상대 경로 기반으로 파일 매핑
                var inputFileMap = CreateFileMap(inputFiles);
                var outputFileMap = CreateFileMap(outputFiles);

                // 입력에만 있는 파일들
                foreach (var inputFile in inputFileMap)
                {
                    if (!outputFileMap.ContainsKey(inputFile.Key))
                    {
                        inputFile.Value.Status = CompareStatus.InputOnly;
                        inputFile.Value.IsComparison = false;
                        differences.Add(inputFile.Value);
                    }
                    else
                    {
                        // 양쪽에 있는 파일 - 내용 비교
                        var outputFile = outputFileMap[inputFile.Key];
                        var isDifferent = CompareFiles(inputFile.Value, outputFile);
                        
                        inputFile.Value.CompareTarget = outputFile;
                        outputFile.CompareTarget = inputFile.Value;
                        
                        if (isDifferent)
                        {
                            inputFile.Value.Status = CompareStatus.Different;
                            inputFile.Value.IsComparison = false;
                            outputFile.Status = CompareStatus.Different;
                            outputFile.IsComparison = false;
                            differences.Add(inputFile.Value);
                        }
                        else
                        {
                            inputFile.Value.Status = CompareStatus.Same;
                            inputFile.Value.IsComparison = true;
                            outputFile.Status = CompareStatus.Same;
                            outputFile.IsComparison = true;
                        }
                    }
                }

                // 출력에만 있는 파일들
                foreach (var outputFile in outputFileMap)
                {
                    if (!inputFileMap.ContainsKey(outputFile.Key))
                    {
                        outputFile.Value.Status = CompareStatus.OutputOnly;
                        outputFile.Value.IsComparison = false;
                        differences.Add(outputFile.Value);
                    }
                }
            });

            return differences;
        }

        /// <summary>
        /// 파일 리스트에서 평면화된 파일만 추출하여 상대경로 기반 맵 생성
        /// </summary>
        private Dictionary<string, FileTreeModel> CreateFileMap(IList<FileTreeModel> fileList)
        {
            var fileMap = new Dictionary<string, FileTreeModel>();
            
            foreach (var file in fileList)
            {
                AddFileToMap(file, fileMap, GetRootPath(fileList));
            }
            
            return fileMap;
        }

        /// <summary>
        /// 재귀적으로 파일을 맵에 추가
        /// </summary>
        private void AddFileToMap(FileTreeModel file, Dictionary<string, FileTreeModel> fileMap, string rootPath)
        {
            if (!file.IsDirectory)
            {
                var relativePath = GetRelativePath(rootPath, file.FilePath);
                fileMap[relativePath] = file;
            }

            foreach (var child in file.Children)
            {
                AddFileToMap(child, fileMap, rootPath);
            }
        }

        /// <summary>
        /// 루트 경로 추출
        /// </summary>
        private string GetRootPath(IList<FileTreeModel> files)
        {
            if (files.Count == 0) return string.Empty;
            
            // 첫 번째 파일의 루트 경로를 기준으로 함
            var firstFile = files.First();
            return Path.GetDirectoryName(firstFile.FilePath) ?? firstFile.FilePath;
        }

        /// <summary>
        /// 상대 경로 계산
        /// </summary>
        private string GetRelativePath(string rootPath, string fullPath)
        {
            try
            {
                var rootUri = new Uri(rootPath.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar);
                var fullUri = new Uri(fullPath);
                return rootUri.MakeRelativeUri(fullUri).ToString().Replace('/', Path.DirectorySeparatorChar);
            }
            catch
            {
                // URI 생성 실패 시 파일명으로 대체
                return Path.GetFileName(fullPath);
            }
        }

        /// <summary>
        /// 두 파일의 내용 비교
        /// </summary>
        private bool CompareFiles(FileTreeModel file1, FileTreeModel file2)
        {
            // 크기가 다르면 다른 파일
            if (file1.FileSize != file2.FileSize)
                return true;

            // 체크섬이 다르면 다른 파일
            if (!string.Equals(file1.Checksum, file2.Checksum, StringComparison.OrdinalIgnoreCase))
                return true;
            
            return false;
        }

        /// <summary>
        /// CompareEntity 형태로 변환 (모든 상태 처리)
        /// </summary>
        public List<CompareEntity> ConvertToCompareEntities(List<FileTreeModel> differences)
        {
            var result = new List<CompareEntity>();

            foreach (var diff in differences)
            {
                var entity = new CompareEntity
                {
                    FileName = diff.FileName,
                    FilePath = diff.FilePath,
                    Checksum = diff.Checksum,
                    FileSize = diff.FileSize ?? 0,
                    LineCount = diff.LineCount,
                    CreateDate = diff.CreateDate,
                    FileType = diff.FileType,
                    Status = diff.Status,
                    IsComparison = diff.IsComparison
                };

                // 상태별 Input/Output 필드 처리
                switch (diff.Status)
                {
                    case CompareStatus.InputOnly:
                        // 입력에만 있는 파일
                        entity.InputFileName = diff.FileName;
                        entity.InputFilePath = diff.FilePath;
                        entity.OutoutFileName = string.Empty;
                        entity.OutoutFilePath = string.Empty;
                        break;

                    case CompareStatus.OutputOnly:
                        // 출력에만 있는 파일
                        entity.InputFileName = string.Empty;
                        entity.InputFilePath = string.Empty;
                        entity.OutoutFileName = diff.FileName;
                        entity.OutoutFilePath = diff.FilePath;
                        break;

                    case CompareStatus.Different:
                        // 양쪽에 있지만 내용이 다른 파일 - 가장 중요!
                        entity.InputFileName = diff.CompareTarget?.FileName ?? diff.FileName;
                        entity.InputFilePath = diff.CompareTarget?.FilePath ?? diff.FilePath;
                        entity.OutoutFileName = diff.FileName;
                        entity.OutoutFilePath = diff.FilePath;
                        break;

                    case CompareStatus.Same:
                        // 양쪽에 동일한 파일 (일반적으로 차이점 목록에 포함 안함)
                        entity.InputFileName = diff.CompareTarget?.FileName ?? diff.FileName;
                        entity.InputFilePath = diff.CompareTarget?.FilePath ?? diff.FilePath;
                        entity.OutoutFileName = diff.FileName;
                        entity.OutoutFilePath = diff.FilePath;
                        break;

                    case CompareStatus.Both:
                        // Both는 비교 가능한 상태 (일반적으로는 Same과 유사하게 처리)
                        entity.InputFileName = diff.CompareTarget?.FileName ?? diff.FileName;
                        entity.InputFilePath = diff.CompareTarget?.FilePath ?? diff.FilePath;
                        entity.OutoutFileName = diff.FileName;
                        entity.OutoutFilePath = diff.FilePath;
                        break;

                    default:
                        // 알 수 없는 상태
                        entity.InputFileName = diff.FileName;
                        entity.InputFilePath = diff.FilePath;
                        entity.OutoutFileName = diff.FileName;
                        entity.OutoutFilePath = diff.FilePath;
                        break;
                }

                result.Add(entity);
            }

            return result;
        }

        /// <summary>
        /// 차이점만 필터링 (Both 상태 제외)
        /// </summary>
        public List<CompareEntity> GetDifferencesOnly(List<FileTreeModel> allResults)
        {
            var differences = allResults.Where(r => 
                r.Status != CompareStatus.Same && 
                r.Status != CompareStatus.Both || 
                !r.IsComparison).ToList();

            return ConvertToCompareEntities(differences);
        }
    }
}