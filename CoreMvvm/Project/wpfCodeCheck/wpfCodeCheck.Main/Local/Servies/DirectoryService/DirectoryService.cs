using System.IO;
using wpfCodeCheck.Main.Local.Models;

namespace wpfCodeCheck.Main.Local.Servies.DirectoryService
{
    public class DirectoryService : IDierctoryFileInfoService
    {
        private readonly IFileCheckSum _fileCheckSum;

        public DirectoryService(IFileCheckSum fileCheckSum)
        {
            _fileCheckSum = fileCheckSum;
        }
        public IEnumerable<CodeInfo> GetDirectoryCodeFileInfos(string path)
        {
            IList<CodeInfo> codeInfos = new List<CodeInfo>();

            DirectoryInfo dirInfo = new DirectoryInfo(path);

            DirectoryInfo[] infos = dirInfo.GetDirectories("*.*", SearchOption.TopDirectoryOnly);

            foreach (DirectoryInfo info in infos)
            {
                string projectName = info.Name;
                if (projectName == ".svn") continue;
                //FileInfo[] fileInfos = new string[] { "*.dll", "*.exe", "*.cxx", "*.cpp", "*.h", "*.cs", "*.xaml", "*.png", "*.config", "*.resx", "*.settings" }
                FileInfo[] fileInfos = new string[] { "*.cxx", "*.cpp", "*.h", "*.cs", "*.xaml", "*.png", "*.config", "*.resx", "*.settings", "*.exe", "*.exe.config", "*.xml", "*.csv", "*.wav" }
                        .SelectMany(i => info.GetFiles(i, SearchOption.AllDirectories))
                        .ToArray();

                foreach (var fi in fileInfos)
                {
                    var fileType = GetFileType(fi.Name);
                    int lineCnt = 0;
                    ulong checkSum = 0;

                    if (fileType != FileDef.Image)
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
            return codeInfos;
        }
        private FileDef GetFileType(string fileName)
        {
            {
                FileDef type = FileDef.Source;
                if (fileName.ToLower().EndsWith(".cs") || fileName.ToLower().EndsWith(".cpp") || fileName.ToLower().EndsWith(".cxx"))
                    type = FileDef.Source;
                else if (fileName.ToLower().EndsWith(".h"))
                    type = FileDef.Header;
                else if (fileName.ToLower().EndsWith(".xaml"))
                    type = FileDef.UI;
                else if (fileName.ToLower().EndsWith(".png"))
                    type = FileDef.Image;
                else if (fileName.ToLower().EndsWith(".exe"))
                    type = FileDef.EXE;
                else if (fileName.ToLower().EndsWith(".dll"))
                    type = FileDef.dll;
                else
                    type = FileDef.Config;

                return type;
            }
        }

    }
}
