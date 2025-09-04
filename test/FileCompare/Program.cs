using wpfCodeCheck.Domain.Services.Interfaces;

internal class Program
{
    private async static Task Main(string[] args)
    {        
        string filePath1 = @"D:\Temp\SW변경이력작성\이전소스파일\HDEV-001\DevTargetManagement";
        string filePath2 = @"D:\Temp\SW변경이력작성\현재소스파일\HDEV-001\DevTargetManagement";

        FileCompare fileCompare = new FileCompare();
        var (list1, list2) = await fileCompare.CompareDirectories(filePath1, filePath2);
        //var index = PrintList(list2, 0);

        var test = FlattenHierarchy(list1);

        //foreach (var item in test)
        //{
        //    Console.WriteLine(item.FilePath);
        //}
        foreach (var item in test)
        {
            Console.WriteLine($"INDEX : {item.Index.ToString()} PATH : {item.FilePath} ");
        }
        //Console.WriteLine(index.ToString());

    }
    private static int PrintList(List<FileCompareModel> list, int index)
    {

        foreach (var item in list)
        {
            if (item.IsComparison == false && item.FileSize != 0)
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
    private static int Index = 1;
    private static List<FileData> FlattenHierarchy(List<FileCompareModel> list)
    {
        
        var flattenedList = new List<FileData>();
        foreach (var item in list)
        {
            if(item.Checksum is not "")
            {
                flattenedList.Add(new FileData()
                {
                    Index = Index ++,
                    Checksum = item.Checksum,
                    FilePath = item.FilePath,
                    FileName = item.FileName,
                    CreateDate = item.CreateDate,
                });
            }       

            if (item.Children != null)
            {
                flattenedList.AddRange(FlattenHierarchy(item.Children));
            }
        }

        return flattenedList;
    }
}

public class FileData
{
    public int Index { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string CreateDate { get; set; } = string.Empty;
    public string Checksum { get; set; } = string.Empty;
}
public class FileCompare
{
    public async Task<(List<FileCompareModel>, List<FileCompareModel>)> CompareDirectories(string path1, string path2)
    {
        var dierctoryFileInfo1 = new FileDirectoryService(new Crc32FileChecSum());
        var dierctoryFileInfo2 = new FileDirectoryService(new Crc32FileChecSum());

        var test1 = await dierctoryFileInfo1.GetDirectoryCodeFileInfosAsync(path1);
        var test2 = await dierctoryFileInfo2.GetDirectoryCodeFileInfosAsync(path2);

        List<string> extensionOrder = new List<string> { "exe", "dll" }; 
        CompareFileItems(test1, test2, path1, path2);
        var sortedFiles1 = SortFilesWithChildren(test1, extensionOrder);
        var sortedFiles2 = SortFilesWithChildren(test2, extensionOrder);

        return (sortedFiles1, sortedFiles2);
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
    private void CompareFileItems(IList<FileCompareModel> list1, IList<FileCompareModel> list2, string basePath1, string basePath2)
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
                if(item1 is not null && item2 is not null)
                {
                    // 두 항목이 같은지 비교
                    if (item1.Equals(item2))
                    {
                        item1.IsComparison = item2.IsComparison = true;
                    }

                    if (item1.Children != null && item2.Children != null)
                    {
                        // 하위 항목 비교
                        CompareFileItems(item1.Children, item2.Children, basePath1, basePath2);
                    }
                }
               
                
            }
            else
            {
                // 한쪽에만 파일이나 폴더가 있는 경우 false
                if (dict1.TryGetValue(path, out item1))
                {
                    item1.IsComparison = false;
                }
                if (dict2.TryGetValue(path, out item2))
                {
                    item2.IsComparison = false;
                }
            }
        }
    }
    private List<FileCompareModel> SortFilesWithChildren(List<FileCompareModel> files, List<string> extensionOrder)
    {
        return files
            .OrderByDescending(file => file.Children != null && file.Children.Any())            
            .ThenBy(file =>
                {
                    if(file.Checksum != "")
                    {
                        int index = extensionOrder.IndexOf(file.FileName.Split(".").Last().ToLower());
                        return index == -1 ? int.MaxValue : index;
                    }
                    return int.MinValue;
                })
            .ThenBy(file => file.FileName)            
            .Select(file =>
            {
                if (file.Children != null && file.Children.Any())
                {
                    file.Children = SortFilesWithChildren(file.Children, extensionOrder);
                }
                return file;
            })
            .ToList();
    }
    private bool CompareFileItemDetails(FileCompareModel item1, FileCompareModel item2)
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
        public async Task<List<FileCompareModel>> GetDirectoryCodeFileInfosAsync(string path)
        {
            List<FileCompareModel> codeInfos = new List<FileCompareModel>();
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException($"The directory '{path}' does not exist.");
            }
            await GetFiles(path, codeInfos, 0);

            return codeInfos.OrderBy(x => x.FileName).ToList();

        }
        private async Task GetFiles(string root, List<FileCompareModel> source, int depth)
        {
            string[] dirs = Directory.GetDirectories(root);
            //string[] files = Directory.GetFiles(root);

            
            FileInfo[] fileInfos = new string[] { "*.*" }
                              .SelectMany(i => GetFilesExcludingFolders(new DirectoryInfo(root), i))
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
        private async Task<FileCompareModel> GetFileCompareModelAsync(FileInfo fi, string projectName)
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

            return new FileCompareModel()
            {
                ProjectName = projectName,
                CreateDate = fi.LastWriteTime.ToString("yyyy-MM-dd HH:mm"),
                FileName = fi.Name,
                FilePath = fi.FullName,
                FileSize = fi.Length,
                LineCount = lineCnt,                
                Checksum = checkSum.ToString("x8").ToLower()
            };
        }
        private IEnumerable<FileInfo> GetFilesExcludingFolders(DirectoryInfo dir, string searchPattern)
        {
            string[] excludeFiles = { "" };
            var excludeFolders = new[] { "Debug", "Release", "bin", "obj", ".svn", ".git", ".vs", "Properties" };
            return dir.EnumerateFiles(searchPattern, SearchOption.TopDirectoryOnly)
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