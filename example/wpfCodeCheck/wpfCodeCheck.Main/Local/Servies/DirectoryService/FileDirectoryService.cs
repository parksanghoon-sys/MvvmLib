using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wpfCodeCheck.Domain.Datas;
using wpfCodeCheck.Domain.Services;
using wpfCodeCheck.Domain.Services.DirectoryServices;

namespace wpfCodeCheck.Main.Local.Servies
{
    public class FileDirectoryService : IDirectoryCompare
    {
        private readonly IFileCheckSum _fileCheckSum;        
        private int _fileIndex = 1;
        public FileDirectoryService(IFileCheckSum fileCheckSum)
        {
            _fileCheckSum = fileCheckSum;            
        }
        public async Task<List<FileCompareModel>> GetDirectoryCodeFileInfosAsync(string path)
        {
            List<FileCompareModel> codeInfos = new List<FileCompareModel>();

            await Task.Run(async () =>
            {
                if (!Directory.Exists(path))
                {
                    throw new DirectoryNotFoundException($"The directory '{path}' does not exist.");
                }
                await GetFiles(path, codeInfos, 0);


            });
            //var sortedFiles = SortFilesWithChildren(codeInfos);

            return codeInfos;
        }
        private List<FileCompareModel> SortFilesWithChildren(List<FileCompareModel> files)
        {
            return files
                .OrderBy(file => file.FileName.EndsWith(".exe") ? 0 :
                                 file.FileName.EndsWith(".dll") ? 1 :
                                 file.FileName.EndsWith(".") ? 2 : 3)
                .ThenBy(file => file.FileName)
                .Select(file =>
                {
                    if (file.Children != null && file.Children.Any())
                    {
                        file.Children = SortFilesWithChildren(file.Children);
                    }
                    return file;
                })
                .ToList();
        }
        private async Task GetFiles(string root, List<FileCompareModel> source, int depth)
        {
            string[] dirs = Directory.GetDirectories(root);
            FileInfo[] fileInfos;
            string[] excludeFiles = { "" };
            var excludeFolders = new[] { "Debug", "Release", "bin", "obj", ".svn", ".git", ".vs" };
            fileInfos = new string[] { "*.*" }
              .SelectMany(i => GetFilesExcludingFolders(new DirectoryInfo(root), i, excludeFiles, excludeFolders))
              .ToArray();


            foreach (string dir in dirs)
            {
                FileCompareModel item = new();
                item.ProjectName = Path.GetFileNameWithoutExtension(dir);
                item.FilePath = dir;
                item.FileSize = null;
                item.Depth = depth;
                item.Children = new();

                source.Add(item);

                await GetFiles(dir, item.Children, depth + 1);
            }

            foreach (var file in fileInfos)
            {
                FileCompareModel item = new();
                item.ProjectName = Path.GetFileNameWithoutExtension(file.FullName);
                item.FileName = file.Name;
                item.FilePath = file.FullName;
                item.FileSize = file.Length;
                item.Depth = depth;
                item.CreateDate = file.LastWriteTime.ToString("yyyy-MM-dd HH:mm");
                item.FileIndex = _fileIndex++;
                byte[] inputBytes;
                ulong checkSum = 0;
                int lineCnt = 0;
                using (FileStream fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                {
                    inputBytes = new byte[fs.Length];
                    await fs.ReadAsync(inputBytes, 0, (int)fs.Length);
                    checkSum = _fileCheckSum.ComputeChecksum(inputBytes);

                    fs.Position = 0;
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        sr.DiscardBufferedData();
                        while (sr.EndOfStream == false)
                        {
                            string text = await sr.ReadLineAsync() ?? string.Empty;
                            lineCnt++;
                        }
                    }
                }
                item.Checksum = checkSum.ToString("x8").ToLower();
                item.LineCount = lineCnt;

                source.Add(item);
            }
        }
        private IEnumerable<FileInfo> GetFilesExcludingFolders(DirectoryInfo dir, string searchPattern, string[] excludeFiles = null, string[] excludeFolders = null)
        {
            return dir.EnumerateFiles(searchPattern, SearchOption.TopDirectoryOnly)
                .Where(file => !excludeFiles.Contains(file.Name, StringComparer.OrdinalIgnoreCase) &&
                               !excludeFolders.Any(ef => file.FullName.Contains($"\\{ef}\\", StringComparison.OrdinalIgnoreCase)));
        }

    }
}
