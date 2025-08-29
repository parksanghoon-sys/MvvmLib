using Microsoft.CodeAnalysis;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using wpfCodeCheck.Domain.Datas;
using wpfCodeCheck.Domain.Models.Base;
using wpfCodeCheck.Domain.Models.Compare;
using wpfCodeCheck.Domain.Enums;

namespace wpfCodeCheck.Domain.Services
{
    public class BaseService : BaseModel, IBaseService        
    {        
        private DiffResultModel<CompareEntityModel>? _compareModel = new(); 

        public DiffResultModel<CompareEntityModel> CompareResult
        {
            get => _compareModel ?? throw new ArgumentNullException(nameof(CompareEntityModel), "CompareResults cannot be null");
            private set => _compareModel = value;
        }
        private Dictionary<EFolderListType, List<Models.Compare.FileCompareModel>> _folderTypeDirectoryFiles = new(2);

        public Dictionary<EFolderListType, List<Models.Compare.FileCompareModel>> FolderTypeDirectoryFiles
        {
            get => _folderTypeDirectoryFiles;            
        }

        public void SetDirectoryCompareReuslt(List<CompareEntityModel> compareResult)
        {
            CompareResult.CompareResults = compareResult;
            OnPropertyChanged(nameof(CompareResult));
        }     
        public void SetFolderTypeDictionaryFiles(EFolderListType eFolderListType, List<Models.Compare.FileCompareModel> fileCompareModels)
        {
            this._folderTypeDirectoryFiles[eFolderListType] = fileCompareModels;
            OnPropertyChanged(nameof(FolderTypeDirectoryFiles));
        }
     
    }
}
