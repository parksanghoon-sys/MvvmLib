using System.ComponentModel;
using wpfCodeCheck.Domain.Datas;
using wpfCodeCheck.Domain.Enums;

namespace wpfCodeCheck.Domain.Services
{
    public interface IBaseService : INotifyPropertyChanged
    {
        DiffReulstModel<CompareEntity> CompareResult { get; }
        void SetDirectoryCompareReuslt(List<CompareEntity> compareResult);
        void SetFolderTypeDictionaryFiles(EFolderListType eFolderListType, List<FileCompareModel> fileCompareModels);
        Dictionary<EFolderListType,List<FileCompareModel>> FolderTypeDirectoryFiles { get; }
    }
}
