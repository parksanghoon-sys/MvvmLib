using CoreMvvmLib.Component.UI.Units;
using System.IO;
using wpfCodeCheck.Domain.Services;
using wpfCodeCheck.Main.Local.Models;

namespace wpfCodeCheck.Main.Local.Servies.DirectoryService
{
    public class SourceDirectoryService : IProjectDirectoryCompare<CodeInfoModel>
    {
        private readonly IFileCheckSum _fileCheckSum;

        public SourceDirectoryService(IFileCheckSum fileCheckSum)
        {
            _fileCheckSum = fileCheckSum;
        }
        public async Task<List<CodeInfoModel>> GetDirectoryCodeFileInfosAsync(string path)
        {
            IList<CodeInfoModel> codeInfos = new List<CodeInfoModel>();
            await Task.Run(async() =>
            {
                if (!Directory.Exists(path))
                {
                    throw new DirectoryNotFoundException($"The directory '{path}' does not exist.");
                }
                
                DirectoryInfo dirInfo = new DirectoryInfo(path);

                DirectoryInfo[] infos = dirInfo.GetDirectories("*.*", SearchOption.TopDirectoryOnly);                

                foreach (DirectoryInfo info in infos)
                {
                    string projectName = info.Name;

                    string[] excludeFiles = { "App.xaml.cs", "App.xaml", "AssemblyInfo.cs", "Resources.Designer.cs", "Settings.Designer.cs, AssemblyAttributes.cs", "ms-persist.xml", "packages.config" };

                    FileInfo[] fileInfos = new string[] { "*.cxx", "*.cpp", "*.h", "*.cs", "*.xaml", "*.config", "*.resx", "*.settings", "*.exe", "*.exe.config", "*.xml", "*.csv", "*.wav" }
                                .SelectMany(i => GetFilesExcludingFolders(info, i, excludeFiles))
                                .ToArray();

                    foreach (var fi in fileInfos)
                    {
                        var codeInfo = await GetCodeInfoModelAsync(fi, projectName);
                        codeInfos.Add(codeInfo);
                    }
                }                
            });
            return codeInfos.OrderBy(x => x.FileName).Distinct(new CodeInfoCompareer()).ToList();

        }
        private async Task<CodeInfoModel> GetCodeInfoModelAsync(FileInfo fi, string projectName)
        {
            int lineCnt = 0;
            ulong checkSum = 0;
            byte[] inputBytes;

            var fileType = GetFileType(fi.Name);

            if (fileType != IconType.Image)
            {
                using (FileStream fs = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read))
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
            }

            return new CodeInfoModel()
            {
                ProjectName = projectName,
                CreateDate = fi.LastWriteTime.ToString("yyyy-MM-dd HH:mm"),
                FileName = fi.Name,
                FilePath = fi.FullName,
                FileSize = fi.Length.ToString(),
                LineCount = lineCnt,
                FileType = fileType,
                Checksum = checkSum.ToString("x8").ToLower()
            };
        }
        private IEnumerable<FileInfo> GetFilesExcludingFolders(DirectoryInfo dir, string searchPattern, string[] excludeFiles = null)
        {
            var excludeFolders = new[] { "Debug", "Release", "bin", "obj", ".svn", ".git", ".vs", "Properties", "LogHelper.Net.Framework" };
            return dir.EnumerateFiles(searchPattern, SearchOption.AllDirectories)
                .Where(file => !excludeFiles.Contains(file.Name, StringComparer.OrdinalIgnoreCase) &&
                               !excludeFolders.Any(ef => file.FullName.Contains($"\\{ef}\\", StringComparison.OrdinalIgnoreCase)));
        }
        private IconType GetFileType(string fileName)
        {
            {
                IconType type = IconType.File;
                if (fileName.ToLower().EndsWith(".cs") || fileName.ToLower().EndsWith(".cpp") || fileName.ToLower().EndsWith(".cxx"))
                    type = IconType.ConsoleLine;
                else if (fileName.ToLower().EndsWith(".h"))
                    type = IconType.File;
                else if (fileName.ToLower().EndsWith(".xaml"))
                    type = IconType.File;
                else if (fileName.ToLower().EndsWith(".png"))
                    type = IconType.Image;
                else if (fileName.ToLower().EndsWith(".exe"))
                    type = IconType.File;
                else if (fileName.ToLower().EndsWith(".dll"))
                    type = IconType.File;
                else
                    type = IconType.Comment;

                return type;
            }
        }

    }
}
