using Microsoft.CodeAnalysis;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using wpfCodeCheck.Domain.Models.Base;
using wpfCodeCheck.Domain.Models;
using wpfCodeCheck.Domain.Enums;
using wpfCodeCheck.Domain.Services.Interfaces;

namespace wpfCodeCheck.Domain.Services
{
    public class BaseService : BaseModel, IBaseService
    {        
        private Dictionary<EFolderListType, List<FileTreeModel>> _folderTypeDictionaryFiles = new();
        private List<CompareEntity> _compareResult = new();        
        public BaseService()
        {
            _folderTypeDictionaryFiles[EFolderListType.INPUT] = new List<FileTreeModel>();
            _folderTypeDictionaryFiles[EFolderListType.OUTPUT] = new List<FileTreeModel>();
        }

        public void SetFolderTypeDictionaryFiles(EFolderListType folderType, List<FileTreeModel> files)
        {
            _folderTypeDictionaryFiles[folderType] = files;
            OnPropertyChanged(nameof(FolderTypeDictionaryFiles));
        }

        public Dictionary<EFolderListType, List<FileTreeModel>> FolderTypeDictionaryFiles 
            => _folderTypeDictionaryFiles;

        public void SetDirectoryCompareReuslt(List<CompareEntity> compareResult)
        {
            _compareResult = compareResult;
            OnPropertyChanged(nameof(DirectoryCompareResult));
        }

        public List<CompareEntity> DirectoryCompareResult => _compareResult;
        public List<CompareEntity> CompareResult => _compareResult;  // 레거시 호환성을 위한 별칭

        public List<FileTreeModel> GetFolderTypeDictionaryFiles(EFolderListType folderType)
        {
            return _folderTypeDictionaryFiles.TryGetValue(folderType, out var files) ? files : new List<FileTreeModel>();
        }      
    }
}
