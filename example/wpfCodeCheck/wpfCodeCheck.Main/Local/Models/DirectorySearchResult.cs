using wpfCodeCheck.Domain.Enums;
using wpfCodeCheck.Domain.Models;

namespace wpfCodeCheck.Main.Local.Models
{
    internal record DirectorySearchResult(EFolderListType type, List<FileTreeModel> fileDatas);

}
