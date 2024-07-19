using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Components;
using CoreMvvmLib.WPF.Components;
using CoreMvvmLib.Core.Services.DialogService;
using wpfCodeCheck.Component.UI.Views;
using CoreMvvmLib.Core.Messenger;
using wpfCodeCheck.Main.Local.Models;
using wpfCodeCheck.Domain.Services;
using wpfCodeCheck.Domain.Enums;

namespace wpfCodeCheck.Main.Local.ViewModels
{
    internal record DirectorySearchResult(EFolderListType type, CustomObservableCollection<CodeInfoModel> fileDatas);
    public partial class FolderListViewModel : ViewModelBase
    {        
        private readonly IDierctoryFileInfoService<CodeInfoModel> _dierctoryFileInfoService;
        private readonly IDialogService _dialogService;

        public FolderListViewModel(IDierctoryFileInfoService<CodeInfoModel> dierctoryFileInfoService, IDialogService dialogService)
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
        private CustomObservableCollection<CodeInfoModel> _fileDatas;
        [Property]
        private CodeInfoModel _fileData;

        //[AsyncRelayCommand]
        //private async Task AsyncFileDialogOpen()
        //{
        //    BrowseForFolderDialog dlg = new BrowseForFolderDialog();
        //    dlg.Title = "Select a folder and click OK!";
        //    dlg.InitialExpandedFolder = @"D:\Project\01.Program\2023\GcsProject\2.FlightSolution";
        //    dlg.OKButtonText = "OK!";
        //    if (true == dlg.ShowDialog())
        //    {
        //        _dialogService.Show(this, nameof(LoadingDialogView), 300, 300);                

        //        FileDatas.Clear();
        //        FolderPath = dlg.SelectedFolder;                

        //        var folderInfoList = await _dierctoryFileInfoService.GetDirectoryCodeFileInfosAsync(FolderPath);

        //        FileDatas.AddRange(folderInfoList);

        //    }
        //    _dialogService.Close(nameof(LoadingDialogView));
        //    WeakReferenceMessenger.Default.Send<CustomObservableCollection<CodeInfo>, FolderCompareViewModel>(FileDatas);
        //}
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
                _dialogService.Show(this, nameof(LoadingDialogView), 300, 300);

                var folderInfoList = await _dierctoryFileInfoService.GetDirectoryCodeFileInfosAsync(FolderPath);

                FileDatas.AddRange(folderInfoList);
                _dialogService.Close(nameof(LoadingDialogView));
                WeakReferenceMessenger.Default.Send<DirectorySearchResult, FolderCompareViewModel>(new DirectorySearchResult(FolderLIstType, FileDatas));
            }
        }
    }
   
}
