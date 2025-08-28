using CoreMvvmLib.Component.UI.Units;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using wpfCodeCheck.Domain.Datas;
using wpfCodeCheck.Domain.Enums;

namespace wpfCodeCheck.Main.Local.Models;

public class CodeInfoModel : BaseNotifyModel
{
    public int FileIndex { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string CreateDate { get; set; } = string.Empty;
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
}
