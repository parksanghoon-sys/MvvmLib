using CoreMvvmLib.Component.UI.Units;
using DocumentFormat.OpenXml.Bibliography;
using wpfCodeCheck.Domain.Datas;
using wpfCodeCheck.Domain.Services;
using wpfCodeCheck.Main.Local.Models;
using wpfCodeCheck.Main.Local.Servies;
using wpfCodeCheck.Main.Local.Servies.DirectoryService;

internal class Program
{
    private async static Task Main(string[] args)
    {        
        string filePath1 = @"D:\Temp\SW변경이력작성\이전소스파일\HDEV-001\FlightSolution";
        string filePath2 = @"D:\Temp\SW변경이력작성\현재소스파일\HDEV-001\FlightSolution";

        FileCompare fileCompare = new FileCompare();
        var (list1, list2) = await fileCompare.CompareDirectories(filePath1, filePath2);
        var index = PrintList(list2, 0);
        Console.WriteLine(index.ToString());
    }
    private static int PrintList(List<CodeInfoModel> list, int index)
    {

        foreach (var item in list)
        {
            if (item.ComparisonResult == false && item.FileSize != "")
            {
                index++;
                Console.WriteLine(item.FilePath);
            }
            if (item.Children != null)
            {
                index = PrintList(item.Children, index);
            }
        }
        return index;
    }
}
public class FileCompare
{
    public async Task<(List<CodeInfoModel>, List<CodeInfoModel>)> CompareDirectories(string path1, string path2)
    {
        var dierctoryFileInfo1 = new FileDirectoryService(new Crc32FileChecSum());
        var dierctoryFileInfo2 = new FileDirectoryService(new Crc32FileChecSum());

        var test1 = await dierctoryFileInfo1.GetDirectoryCodeFileInfosAsync(path1);
        var test2 = await dierctoryFileInfo2.GetDirectoryCodeFileInfosAsync(path2);


        CompareFileItems(test1, test2, path1, path2);

        return (test1, test2);
    }

    private string GetRelativePath(string fullPath, string basePath)
    {
        // basePath 이후의 경로 추출
        if (fullPath.StartsWith(basePath))
        {
            return fullPath.Substring(basePath.Length).TrimStart(Path.DirectorySeparatorChar);
        }
        return fullPath;
    }
    private void CompareFileItems(List<CodeInfoModel> list1, List<CodeInfoModel> list2, string basePath1, string basePath2)
    {
        
        var dict1 = list1?.ToDictionary(
            item => GetRelativePath(item.FilePath, basePath1));
        var dict2 = list2?.ToDictionary(
            item => GetRelativePath(item.FilePath, basePath2));

        var allPaths = dict1?.Keys.Union(dict2?.Keys).ToList();

        foreach (var path in allPaths)
        {
            if (dict1.TryGetValue(path, out var item1) && dict2.TryGetValue(path, out var item2))
            {
                // 두 항목이 같은지 비교
                if(item1.Equals(item2))
                {
                    item1.ComparisonResult = item2.ComparisonResult = true;
                }
                

                if (item1.Children != null && item2.Children != null)
                {
                    // 하위 항목 비교
                    CompareFileItems(item1.Children, item2.Children, basePath1, basePath2);
                }
                
            }
            else
            {
                // 한쪽에만 파일이나 폴더가 있는 경우 false
                if (dict1.TryGetValue(path, out item1))
                {
                    item1.ComparisonResult = false;
                }
                if (dict2.TryGetValue(path, out item2))
                {
                    item2.ComparisonResult = false;
                }
            }
        }
    }

    private bool CompareFileItemDetails(CodeInfoModel item1, CodeInfoModel item2)
    {
        return item1.Checksum == item2.Checksum &&
               item1.CreateDate == item2.CreateDate;
    }
    public class FileDirectoryService
    {
        private readonly IFileCheckSum _fileCheckSum;

        public FileDirectoryService(IFileCheckSum fileCheckSum)
        {
            _fileCheckSum = fileCheckSum;
        }
        public async Task<List<CodeInfoModel>> GetDirectoryCodeFileInfosAsync(string path)
        {
            List<CodeInfoModel> codeInfos = new List<CodeInfoModel>();
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException($"The directory '{path}' does not exist.");
            }
            await GetFiles(path, codeInfos, 0);

            return codeInfos.OrderBy(x => x.FileName).ToList();

        }
        private async Task GetFiles(string root, List<CodeInfoModel> source, int depth)
        {
            string[] dirs = Directory.GetDirectories(root);
            string[] files = Directory.GetFiles(root);

            foreach (string dir in dirs)
            {
                CodeInfoModel item = new();
                item.ProjectName = Path.GetFileNameWithoutExtension(dir);
                item.FilePath = dir;
                item.FileSize = "";
                item.FileType = GetFileType(item.FilePath);
                item.Depth = depth;
                item.Children = new();

                source.Add(item);

                await GetFiles(dir, item.Children, depth + 1);
            }

            foreach (string file in files)
            {
                var fileitem = new FileInfo(file);
                CodeInfoModel item = new();
                item.ProjectName = Path.GetFileNameWithoutExtension(file);
                item.FilePath = fileitem.FullName;
                item.FileSize = fileitem.Length.ToString();
                item.FileType = GetFileType(item.FilePath);
                item.Depth = depth;
                item.CreateDate = fileitem.LastWriteTime.ToString("yyyy-MM-dd HH:mm");
                byte[] inputBytes;
                ulong checkSum = 0;
                int lineCnt = 0;
                using (FileStream fs = new FileStream(fileitem.FullName, FileMode.Open, FileAccess.Read))
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