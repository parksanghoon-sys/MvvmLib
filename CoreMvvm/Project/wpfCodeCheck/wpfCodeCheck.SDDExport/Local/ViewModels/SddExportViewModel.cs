using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Components;
using CoreMvvmLib.WPF.Components;
using wpfCodeCheck.Domain.Services;
using wpfCodeCheck.SDDExport.Local.Models;

namespace wpfCodeCheck.SDDExport.Local.ViewModels
{
    public partial class SddExportViewModel : ViewModelBase
    {
        private readonly IBaseService _baseService;

        public SddExportViewModel(IBaseService baseService)
        {
            _baseService = baseService;
            SetFileData();
        }
        [Property]
        private CustomObservableCollection<FileInfoModel> _fileDatas = new();

        private void SetFileData()
        {
            foreach (var file in _baseService.FolderTypeDirectoryFiles[Domain.Enums.EFolderListType.OUTPUT])
            {
                FileDatas.Add(new FileInfoModel()
                {
                    Checksum = file.Checksum,
                    LineCount = file.LineCount,
                    CreateDate = file.CreateDate,
                    Description = file.Description,
                    FileName = file.FileName,
                    FilePath = file.FilePath,
                    FileSize = file.FileSize,                    
                });
            }
        }
    }
}
