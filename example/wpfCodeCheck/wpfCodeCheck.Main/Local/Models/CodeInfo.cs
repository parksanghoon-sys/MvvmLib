using CoreMvvmLib.Design.Enums;
using System.IO;
using wpfCodeCheck.Domain.Enums;
using wpfCodeCheck.Domain.Models;
using wpfCodeCheck.Domain.Models.Base;
using static Microsoft.CodeAnalysis.CSharp.SyntaxTokenParser;

namespace wpfCodeCheck.Main.Local.Models;

public class FileInfoDto : BaseModel
{
    public int FileIndex { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public DateTime CreateDate { get; set; }
    public long? FileSize { get; set; }
    public int LineCount { get; set; }
    public string Checksum { get; set; } = string.Empty;
    public IconType FileIcon => GetFileType(FileName);
    public EFileType FileType {  get; set; }

    private bool _isComparison;
    public bool IsComparison
    {
        get => _isComparison;
        set
        {
            _isComparison = value;
            OnPropertyChanged();
        }
    }
    private IconType GetFileType(string fileName)
    {
        IconType type = IconType.File;
        if (fileName.ToLower().EndsWith(".cs") || fileName.ToLower().EndsWith(".cpp") || fileName.ToLower().EndsWith(".cxx"))
            type = IconType.Code;
        else if (fileName.ToLower().EndsWith(".h"))
            type = IconType.File;
        else if (fileName.ToLower().EndsWith(".xaml"))
            type = IconType.ConsoleLine;
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
    private int _fileIndex = 1;
    public async Task<IEnumerable<FileInfoDto>> FileTreeModelToFileInfo(List<FileTreeModel> list)
    {
        List<FileInfoDto> fileList = new List<FileInfoDto>();
        await Parallel.ForEachAsync(list, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, async (file, token) =>
        {
            if (!file.IsDirectory)
            {
                // 파일인 경우만 CodeInfoModel로 변환하여 추가
                fileList.Add(new FileInfoDto()
                {
                    Checksum = file.Checksum,
                    FilePath = file.FilePath,
                    FileName = file.FileName,
                    CreateDate = file.CreateDate,
                    LineCount = file.LineCount,
                    IsComparison = file.IsComparison,
                    FileSize = file.FileSize,
                    FileIndex = _fileIndex++,
                    FileType = file.FileType,
                });
            }

            if (file.Children != null && file.Children.Any())
            {
                FileTreeModelToFileInfo(file.Children);
            }
        });

        return fileList;
    }
}
