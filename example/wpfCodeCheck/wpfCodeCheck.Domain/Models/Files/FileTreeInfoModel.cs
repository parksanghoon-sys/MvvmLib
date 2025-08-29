using CoreMvvmLib.Component.UI.Units;

namespace wpfCodeCheck.Domain.Models.Files
{
    public class FileTreeInfoModel
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public int Depth { get; set; }
        public List<FileTreeInfoModel> Children { get; set; } = new List<FileTreeInfoModel>();
        public IconType FileType => GetFileType(FileName);

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
}