using CoreMvvmLib.Component.UI.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wpfCodeCheck.Domain.Datas;

namespace wpfCodeCheck.SDDExport.Local.Models
{
    public class FileInfoModel : FileItem
    {
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
