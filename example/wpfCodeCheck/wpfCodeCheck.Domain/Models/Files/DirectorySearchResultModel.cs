using wpfCodeCheck.Domain.Enums;
using wpfCodeCheck.Domain.Models.Compare;

namespace wpfCodeCheck.Domain.Models.Files
{
    public record DirectorySearchResultModel(EFolderListType Type, List<FileCompareModel> FileDatas);
}