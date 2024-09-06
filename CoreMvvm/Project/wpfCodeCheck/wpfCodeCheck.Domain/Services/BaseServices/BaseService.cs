using Microsoft.CodeAnalysis;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using wpfCodeCheck.Domain.Datas;
using wpfCodeCheck.Domain.Enums;

namespace wpfCodeCheck.Domain.Services
{
    public class BaseService : BaseNotifyModel, IBaseService        
    {        
        private DiffReulstModel<CompareEntity>? _compareModel = new(); 

        public DiffReulstModel<CompareEntity> CompareResult
        {
            get => _compareModel ?? throw new ArgumentNullException(nameof(CompareEntity), "CompareResults cannot be null");
            private set => _compareModel = value;
        }
        private Dictionary<EFolderListType, List<FileCompareModel>> _folderTypeDirectoryFiles = new(2);

        public Dictionary<EFolderListType, List<FileCompareModel>> FolderTypeDirectoryFiles
        {
            get => _folderTypeDirectoryFiles;            
        }

        public void SetDirectoryCompareReuslt(List<CompareEntity> compareResult)
        {
            CompareResult.CompareResults = compareResult;
            OnPropertyChanged(nameof(CompareResult));
        }     
        public void SetFolderTypeDictionaryFiles(EFolderListType eFolderListType, List<FileCompareModel> fileCompareModels)
        {
            this._folderTypeDirectoryFiles.Add(eFolderListType, fileCompareModels);
            OnPropertyChanged(nameof(FolderTypeDirectoryFiles));
        }
     
    }
}
