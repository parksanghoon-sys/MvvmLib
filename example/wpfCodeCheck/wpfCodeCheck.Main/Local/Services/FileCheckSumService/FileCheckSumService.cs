using System.Security.Cryptography;
using wpfCodeCheck.Domain.Models;
using wpfCodeCheck.Domain.Services.Interfaces;
using System.IO;
using wpfCodeCheck.Domain.Enums;

namespace wpfCodeCheck.Main.Local.Servies.FileCheckSumService
{
    public class Sha256FileCheckSumService : IFileCheckSum
    {
        private readonly string[] _codeFileExtensions = { ".cs", ".cpp", ".h", ".hpp", ".c", ".cc", ".cxx" };
        
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

        public uint ComputeChecksum(byte[] input)
        {
            if (input == null || input.Length == 0)
                return 0;

            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(input);

                // 만약 string 대신 uint 리턴이라면, 해시 결과의 일부를 uint로 변환
                // (여기서는 해시 앞 4바이트를 uint로 해석)
                return BitConverter.ToUInt32(hashBytes, 0);
            }
        }

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

                        // Recursively get children
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
                    // Handle exceptions silently
                }
            });

            return result;
        }

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