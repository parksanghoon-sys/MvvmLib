using wpfCodeCheck.Domain.Models;
using wpfCodeCheck.Domain.Enums;

namespace wpfCodeCheck.Domain.Services
{
    /// <summary>
    /// 디렉토리 검색과 파일 비교를 담당하는 통합 서비스
    /// </summary>
    public class FileTreeService
    {
        /// <summary>
        /// 지정된 디렉토리에서 파일 트리를 생성
        /// </summary>
        public async Task<FileTreeModel> BuildFileTreeAsync(string rootPath)
        {
            if (!Directory.Exists(rootPath))
                throw new DirectoryNotFoundException($"Directory not found: {rootPath}");

            var root = new FileTreeModel
            {
                FilePath = rootPath,
                FileName = Path.GetFileName(rootPath) ?? rootPath,
                Depth = 0
            };

            await Task.Run(() => BuildFileTreeRecursive(root, rootPath, 0));
            return root;
        }

        /// <summary>
        /// 재귀적으로 파일 트리 생성
        /// </summary>
        private void BuildFileTreeRecursive(FileTreeModel parent, string currentPath, int depth)
        {
            try
            {
                // 디렉토리 내의 모든 항목 가져오기
                var entries = Directory.GetFileSystemEntries(currentPath);

                foreach (var entry in entries.OrderBy(e => e))
                {
                    var isDirectory = Directory.Exists(entry);
                    var fileName = Path.GetFileName(entry);

                    var fileModel = new FileTreeModel
                    {
                        FilePath = entry,
                        FileName = fileName,
                        Depth = depth + 1,
                        Parent = parent
                    };

                    if (!isDirectory)
                    {
                        // 파일 정보 설정
                        var fileInfo = new FileInfo(entry);
                        fileModel.FileSize = fileInfo.Length;
                        fileModel.CreateDate = fileInfo.CreationTime.ToString("yyyy-MM-dd HH:mm:ss");
                        fileModel.LineCount = GetLineCount(entry);
                        fileModel.Checksum = CalculateChecksum(entry);
                    }

                    parent.Children.Add(fileModel);

                    // 디렉토리인 경우 재귀 호출
                    if (isDirectory)
                    {
                        BuildFileTreeRecursive(fileModel, entry, depth + 1);
                    }
                }
            }
            catch (Exception ex)
            {
                // 접근 권한 등의 문제로 실패한 경우 로그만 남기고 계속 진행
                Console.WriteLine($"Error processing directory {currentPath}: {ex.Message}");
            }
        }

        /// <summary>
        /// 두 파일 트리를 비교하여 차이점을 찾음
        /// </summary>
        public async Task<List<FileTreeModel>> CompareFileTreesAsync(FileTreeModel inputTree, FileTreeModel outputTree)
        {
            var differences = new List<FileTreeModel>();

            await Task.Run(() =>
            {
                // 입력 트리의 모든 파일 가져오기
                var inputFiles = GetAllFiles(inputTree).ToDictionary(f => GetRelativePath(inputTree.FilePath, f.FilePath), f => f);
                var outputFiles = GetAllFiles(outputTree).ToDictionary(f => GetRelativePath(outputTree.FilePath, f.FilePath), f => f);

                // 입력에만 있는 파일들
                foreach (var inputFile in inputFiles)
                {
                    if (!outputFiles.ContainsKey(inputFile.Key))
                    {
                        inputFile.Value.Status = CompareStatus.InputOnly;
                        inputFile.Value.IsComparison = false;
                        differences.Add(inputFile.Value);
                    }
                    else
                    {
                        // 양쪽에 있는 파일 - 내용 비교
                        var outputFile = outputFiles[inputFile.Key];
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
                            inputFile.Value.Status = CompareStatus.Both;
                            inputFile.Value.IsComparison = true;
                            outputFile.Status = CompareStatus.Both;
                            outputFile.IsComparison = true;
                        }
                    }
                }

                // 출력에만 있는 파일들
                foreach (var outputFile in outputFiles)
                {
                    if (!inputFiles.ContainsKey(outputFile.Key))
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
        /// 트리에서 모든 파일(디렉토리 제외) 가져오기
        /// </summary>
        private IEnumerable<FileTreeModel> GetAllFiles(FileTreeModel root)
        {
            if (!root.IsDirectory)
                yield return root;

            foreach (var child in root.Children)
            {
                foreach (var file in GetAllFiles(child))
                {
                    yield return file;
                }
            }
        }

        /// <summary>
        /// 상대 경로 계산
        /// </summary>
        private string GetRelativePath(string rootPath, string fullPath)
        {
            var rootUri = new Uri(rootPath.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar);
            var fullUri = new Uri(fullPath);
            return rootUri.MakeRelativeUri(fullUri).ToString().Replace('/', Path.DirectorySeparatorChar);
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
            if (file1.Checksum != file2.Checksum)
                return true;
            
            // 추가적인 내용 비교가 필요한 경우 여기서 수행
            return false;
        }

        /// <summary>
        /// 파일의 라인 수 계산
        /// </summary>
        private int GetLineCount(string filePath)
        {
            try
            {
                return File.ReadAllLines(filePath).Length;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 파일의 체크섬 계산 (간단한 MD5)
        /// </summary>
        private string CalculateChecksum(string filePath)
        {
            try
            {
                using var md5 = System.Security.Cryptography.MD5.Create();
                using var stream = File.OpenRead(filePath);
                var hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}