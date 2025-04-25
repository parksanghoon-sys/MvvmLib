using wpfCodeCheck.Domain.Enums;
using wpfCodeCheck.Domain.Datas;

namespace wpfCodeCheck.Main.Local.Models
{
    internal record DirectorySearchResult(EFolderListType type, List<FileCompareModel> fileDatas);

}
