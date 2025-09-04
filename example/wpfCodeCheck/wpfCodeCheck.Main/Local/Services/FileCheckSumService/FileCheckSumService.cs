using System.Security.Cryptography;
using wpfCodeCheck.Domain.Models;
using wpfCodeCheck.Domain.Services.Interfaces;
using System.IO;
using wpfCodeCheck.Domain.Enums;

namespace wpfCodeCheck.Main.Local.Servies.FileCheckSumService
{
    /// <summary>
    /// SHA-256 알고리즘을 사용하여 파일 체크섬을 계산하는 서비스
    /// 디렉토리 탐색, 코드 파일 분석, 체크섬 계산 기능을 제공합니다.
    /// </summary>
    public class Sha256FileCheckSumService : IFileCheckSum
    {
        /// <summary>
        /// 코드 파일로 인식할 파일 확장자 목록
        /// </summary>
        private readonly string[] _codeFileExtensions = { ".cs", ".cpp", ".h", ".hpp", ".c", ".cc", ".cxx" };
        
        /// <summary>
        /// 비동기적으로 파일의 SHA-256 체크섬을 계산합니다.
        /// </summary>
        /// <param name="filePath">체크섬을 계산할 파일 경로</param>
        /// <returns>Base64 인코딩된 체크섬 문자열</returns>
        public async Task<string> CalculateChecksumAsync(string filePath)
        {
            if (!File.Exists(filePath))
                return string.Empty;

            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                using (var sha256 = SHA256.Create())
                {
                    var hashBytes = await Task.Run(() => sha256.ComputeHash(fileStream));
                    return Convert.ToBase64String(hashBytes);
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 바이트 배열로부터 체크섬을 계산하여 uint로 반환합니다.
        /// </summary>
        /// <param name="input">체크섬을 계산할 바이트 배열</param>
        /// <returns>SHA-256 해시의 처음 4바이트를 uint로 변환한 값</returns>
        public uint ComputeChecksum(byte[] input)
        {
            if (input == null || input.Length == 0)
                return 0;

            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(input);

                // 해시의 처음 4바이트를 uint로 변환
                return BitConverter.ToUInt32(hashBytes, 0);
            }
        }

        /// <summary>
        /// 비동기적으로 디렉토리를 재귀 탐색하여 파일 트리 모델을 생성합니다.
        /// 코드 파일의 경우 체크섬과 라인 수를 자동으로 계산합니다.
        /// </summary>
        /// <param name="directoryPath">탐색할 디렉토리 경로</param>
        /// <returns>파일 및 디렉토리 정보가 포함된 트리 모델 목록</returns>
        public async Task<List<FileTreeModel>> GetDirectoryFileInfosAsync(string directoryPath)
        {
            var result = new List<FileTreeModel>();
            
            if (!Directory.Exists(directoryPath))
                return result;

            await Task.Run(() =>
            {
                try
                {
                    var directories = Directory.GetDirectories(directoryPath);
                    var files = Directory.GetFiles(directoryPath);

                    foreach (var dir in directories)
                    {
                        var dirInfo = new DirectoryInfo(dir);
                        var dirModel = new FileTreeModel
                        {
                            FilePath = dir,
                            FileName = dirInfo.Name,                            
                            CreateDate = dirInfo.CreationTime.ToString("yyyy-MM-dd HH:mm"),
                            FileType = EFileType.DIRECTORY
                        };

                        // 재귀적으로 하위 디렉토리 탐색
                        var children = GetDirectoryFileInfosAsync(dir).Result;
                        dirModel.Children.AddRange(children);
                        
                        result.Add(dirModel);
                    }

                    foreach (var file in files)
                    {
                        var fileInfo = new FileInfo(file);
                        var fileModel = new FileTreeModel
                        {
                            FilePath = file,
                            FileName = fileInfo.Name,                            
                            FileSize = fileInfo.Length,
                            CreateDate = fileInfo.CreationTime.ToString("yyyy-MM-dd HH:mm"),
                            FileType = EFileType.FILE
                        };

                        if (_codeFileExtensions.Any(ext => file.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                        {
                            fileModel.Checksum = CalculateChecksumAsync(file).Result;
                            fileModel.LineCount = CountLinesInFile(file);
                        }

                        result.Add(fileModel);
                    }
                }
                catch
                {
                    // 예외 발생 시 조용히 처리
                }
            });

            return result;
        }

        /// <summary>
        /// 파일의 라인 수를 계산합니다.
        /// </summary>
        /// <param name="filePath">라인 수를 계산할 파일 경로</param>
        /// <returns>파일의 라인 수, 오류 시 0</returns>
        private int CountLinesInFile(string filePath)
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
    }
}