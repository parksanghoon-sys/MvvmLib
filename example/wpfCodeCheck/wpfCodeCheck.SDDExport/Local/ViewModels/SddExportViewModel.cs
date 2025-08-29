using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Components;
using CoreMvvmLib.WPF.Components;
using wpfCodeCheck.Domain.Models;
using wpfCodeCheck.Domain.Services.Interfaces;

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
        private CustomObservableCollection<FileTreeModel> _fileDatas = new();

        private void SetFileData()
        {
            if (_baseService.OutputFiles != null)
            {
                _fileDatas.AddRange(_baseService.OutputFiles);             
            }
        }     

    }
}
