using CoreMvvmLib.Component.UI.Units;
using System.IO;
using System.Linq;
using System.Resources;
using wpfCodeCheck.Main.Local.Models;

namespace wpfCodeCheck.Main.Local.Servies.DirectoryService
{
    public class DirectoryManager : IDierctoryFileInfoService
    {
        private readonly IFileCheckSum _fileCheckSum;

        public DirectoryManager(IFileCheckSum fileCheckSum)
        {
            _fileCheckSum = fileCheckSum;
        }
        public Task<List<CodeInfo>> GetDirectoryCodeFileInfosAsync(string path)
        {
            return Task.Run(() =>
            {
                if (!Directory.Exists(path))
                {
                    throw new DirectoryNotFoundException($"The directory '{path}' does not exist.");
                }
                IList<CodeInfo> codeInfos = new List<CodeInfo>();

                DirectoryInfo dirInfo = new DirectoryInfo(path);

                DirectoryInfo[] infos = dirInfo.GetDirectories("*.*", SearchOption.TopDirectoryOnly);

                foreach (DirectoryInfo info in infos)
                {
                    string projectName = info.Name;
                    //if (projectName.Contains(".svn")) continue;
                    //if (projectName.Contains(".git")) continue;
                    //if (projectName == ".vs") continue;
                    //if (projectName == "bin") continue;
                    //if (projectName == "obj") continue;
                    //if (projectName == "Debug") continue;
                    //if (projectName == "Release") continue;
                    //if (projectName == "Properties") continue;
                    string[] excludeFiles = { "App.xaml.cs", "App.xaml", "AssemblyInfo.cs", "Resources.Designer.cs", "Settings.Designer.cs, AssemblyAttributes.cs", "ms-persist.xml", "packages.config" };
                    //FileInfo[] fileInfos = new string[] { "*.dll", "*.exe", "*.cxx", "*.cpp", "*.h", "*.cs", "*.xaml", "*.png", "*.config", "*.resx", "*.settings" }
                    //FileInfo[] fileInfos = new string[] { "*.cxx", "*.cpp", "*.h", "*.cs", "*.xaml", /*"*.png",*/ "*.config", "*.resx", "*.settings", "*.exe", "*.exe.config", "*.xml", "*.csv", "*.wav" }
                    //        .SelectMany(i => info.GetFiles(i, SearchOption.AllDirectories))                            
                    //        .Where(file => !excludeFiles.Contains(file.Name, StringComparer.OrdinalIgnoreCase))
                    //        .ToArray();
                    FileInfo[] fileInfos = new string[] { "*.cxx", "*.cpp", "*.h", "*.cs", "*.xaml", "*.config", "*.resx", "*.settings", "*.exe", "*.exe.config", "*.xml", "*.csv", "*.wav" }
                                .SelectMany(i => GetFilesExcludingFolders(info, i, excludeFiles))
                                .ToArray();

                    foreach (var fi in fileInfos)
                    {
                        var fileType = GetFileType(fi.Name);
                        int lineCnt = 0;
                        ulong checkSum = 0;

                        if (fileType != IconType.Image)
                        {
                            using (FileStream fs1 = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read))
                            {
                                using (StreamReader sr = new StreamReader(fs1))
                                {
                                    checkSum = _fileCheckSum.Calculate(sr.ReadToEnd());
                                    fs1.Position = 0;
                                    sr.DiscardBufferedData();
                                    while (sr.EndOfStream == false)
                                    {
                                        string text = sr.ReadLine();
                                        lineCnt++;
                                    }
                                }
                            }
                        }
                        codeInfos.Add(new CodeInfo()
                        {
                            ProjectName = projectName,
                            CreateDate = fi.LastWriteTime.ToString("yyyy-MM-dd"), //fi.CreationTime.ToString("yyyy-MM-dd"),
                            FileName = fi.Name,
                            FilePath = fi.Directory.ToString(),
                            FileSize = fi.Length > 1000 ? string.Format("{0}KB", (double)(fi.Length / 1000)) : string.Format("{0}B", fi.Length),
                            LineCount = lineCnt,
                            FileType = fileType,
                            Checksum = checkSum.ToString("x").ToUpper()
                        });
                    }

                }                
                return codeInfos.OrderBy(x=>x.FileName).Distinct(new CodeInfoCompareer()).ToList();
            });
           
        }
        private IEnumerable<FileInfo> GetFilesExcludingFolders(DirectoryInfo dir, string searchPattern, string[] excludeFiles)
        {
            var excludeFolders = new[] { "Debug", "Release", "bin", "obj", ".svn", ".git", ".vs", "Properties" };
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
