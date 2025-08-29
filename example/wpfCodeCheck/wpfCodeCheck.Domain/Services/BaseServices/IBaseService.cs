using System.ComponentModel;
using wpfCodeCheck.Domain.Datas;
using wpfCodeCheck.Domain.Models.Compare;
using wpfCodeCheck.Domain.Enums;

namespace wpfCodeCheck.Domain.Services
{
    public interface IBaseService : INotifyPropertyChanged
    {
        DiffResultModel<CompareEntityModel> CompareResult { get; }
        void SetDirectoryCompareReuslt(List<CompareEntityModel> compareResult);
        void SetFolderTypeDictionaryFiles(EFolderListType eFolderListType, List<Models.Compare.FileCompareModel> fileCompareModels);
        Dictionary<EFolderListType,List<Models.Compare.FileCompareModel>> FolderTypeDirectoryFiles { get; }
    }
}
