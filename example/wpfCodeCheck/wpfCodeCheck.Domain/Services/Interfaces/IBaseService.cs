using System.ComponentModel;
using wpfCodeCheck.Domain.Models;
using wpfCodeCheck.Domain.Enums;

namespace wpfCodeCheck.Domain.Services.Interfaces;

public interface IBaseService : INotifyPropertyChanged
{
    Dictionary<EFolderListType, List<FileTreeModel>> FolderTypeDictionaryFiles { get; }   
    List<CompareEntity> DirectoryCompareResult { get; }
    List<CompareEntity> CompareResult { get; }  // 레거시 호환성을 위한 별칭

    void SetFolderTypeDictionaryFiles(EFolderListType folderType, List<FileTreeModel> files);
    void SetDirectoryCompareReuslt(List<CompareEntity> compareResult);
    List<FileTreeModel> GetFolderTypeDictionaryFiles(EFolderListType folderType);
}
