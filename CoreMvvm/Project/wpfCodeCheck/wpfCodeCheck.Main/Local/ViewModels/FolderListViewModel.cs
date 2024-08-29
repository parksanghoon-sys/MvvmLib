using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Components;
using CoreMvvmLib.WPF.Components;
using CoreMvvmLib.Core.Services.DialogService;
using wpfCodeCheck.Component.UI.Views;
using CoreMvvmLib.Core.Messenger;
using wpfCodeCheck.Main.Local.Models;
using wpfCodeCheck.Domain.Services;
using wpfCodeCheck.Domain.Enums;
using System.Windows;
using System.Windows.Threading;
using wpfCodeCheck.Domain.Datas;

namespace wpfCodeCheck.Main.Local.ViewModels
{
    internal record DirectorySearchResult(EFolderListType type, CustomObservableCollection<CodeInfoModel> fileDatas);
    public partial class FolderListViewModel : ViewModelBase
    {        
        private readonly IProjectDirectoryCompare<CodeInfoModel> _dierctoryFileInfoService;
        private readonly IDialogService _dialogService;

        public FolderListViewModel(IProjectDirectoryCompare<CodeInfoModel> dierctoryFileInfoService, IDialogService dialogService)
        {           
            _dierctoryFileInfoService = dierctoryFileInfoService;
            _dialogService = dialogService;
            FileDatas = new();

            WeakReferenceMessenger.Default.Register<FolderListViewModel, EFolderCompareList>(this, OnReceiveClearMessage);            
        }
        private string? _folerPath;

        public string? FolderPath
        {
            get => _folerPath;
            set 
            { 
                SetProperty(ref _folerPath,value);
                OnFolderPathChanged();
            }
        }      
        [Property]
        private EFolderListType _folderLIstType;
        [Property]
        private CustomObservableCollection<CodeInfoModel> _fileDatas = new();
        [Property]
        private FileItem _fileData = new();
        
        private void OnReceiveClearMessage(FolderListViewModel model, EFolderCompareList list)
        {
            if (EFolderCompareList.CLEAR == list)
                this.FileDatas.Clear();
        }
        private async void OnFolderPathChanged()
        {
            if (string.IsNullOrEmpty(FolderPath) == false)
            {
                await GetDirectoryFilesInfoAsync();                
            }
        }
        private async Task GetDirectoryFilesInfoAsync()
        {
            if (string.IsNullOrEmpty(FolderPath) == false)
            {
                var dd = Application.Current.MainWindow;
                _dialogService.Show(this, typeof(LoadingDialogView), 300, 300);
                var folderInfoList = await _dierctoryFileInfoService.GetDirectoryCodeFileInfosAsync(FolderPath);

                await FileDatas.AddItemsAsync(folderInfoList);
                //folderInfoList.ForEach(item =>  FileDatas.Add(item));
                _dialogService.Close(typeof(LoadingDialogView));
                WeakReferenceMessenger.Default.Send<DirectorySearchResult, FolderCompareViewModel>(new DirectorySearchResult(FolderLIstType, FileDatas));
            }            
        }
    }
   
}
