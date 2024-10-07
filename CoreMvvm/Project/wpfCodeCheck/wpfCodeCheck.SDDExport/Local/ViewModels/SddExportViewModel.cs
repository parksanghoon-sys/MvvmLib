using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Components;
using CoreMvvmLib.WPF.Components;
using wpfCodeCheck.Domain.Datas;
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
                    FileName = file.FileName == "" ? file.ProjectName : file.FileName,
                    FilePath = file.FilePath,                                        
                    Depth = file.Depth,                    
                });
                if (file.Children != null)
                {
                    AddFileDatas(FileDatas.Last(),file);
                }
            }
        }
        private void AddFileDatas(FileInfoModel file, FileCompareModel model)
        {
            if (model.Children != null)
            {
                foreach (var child in model.Children)
                {
                    file.Children.Add(new FileInfoModel()
                    {
                        FileName = child.FileName == "" ? child.ProjectName : child.FileName,
                        FilePath = child.FilePath,                        
                        Depth = child.Depth,
                    });
                    if (child.Children != null)
                    {
                        AddFileDatas(file.Children.Last(), child);
                    }
                }
            }
        }

    }
}
