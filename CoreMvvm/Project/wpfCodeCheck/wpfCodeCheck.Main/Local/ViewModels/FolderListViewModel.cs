using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Components;
using wpfCodeCheck.Main.Local.Servies.DirectoryService;
using wpfCodeCheck.Main.UI.Units;
using CoreMvvmLib.WPF.Components;
using CoreMvvmLib.Core.Services.DialogService;
using wpfCodeCheck.Shared.UI.Views;
using CoreMvvmLib.Core.Messenger;
using wpfCodeCheck.Shared.Local.Models;

namespace wpfCodeCheck.Main.Local.ViewModels
{
    public partial class FolderListViewModel : ViewModelBase
    {
        private readonly IDierctoryFileInfoService _dierctoryFileInfoService;
        private readonly IDialogService _dialogService;

        public FolderListViewModel(IDierctoryFileInfoService dierctoryFileInfoService, IDialogService dialogService)
        {           
            _dierctoryFileInfoService = dierctoryFileInfoService;
            _dialogService = dialogService;
            FileDatas = new();
        }

        [Property]
        private string _folderPath;
        [Property]
        private CustomObservableCollection<CodeInfo> _fileDatas;
        [Property]
        private CodeInfo _fileData;

        [AsyncRelayCommand]
        private async Task AsyncFileDialogOpen()
        {
            BrowseForFolderDialog dlg = new BrowseForFolderDialog();
            dlg.Title = "Select a folder and click OK!";
            dlg.InitialExpandedFolder = @"D:\Project\01.Program\2023\GcsProject\2.FlightSolution";
            dlg.OKButtonText = "OK!";
            if (true == dlg.ShowDialog())
            {
                _dialogService.Show(this, nameof(LoadingDialogView), 300, 300);                

                FileDatas.Clear();
                FolderPath = dlg.SelectedFolder;

                var folderInfoList = await _dierctoryFileInfoService.GetDirectoryCodeFileInfosAsync(FolderPath);

                FileDatas.AddRange(folderInfoList);

            }            
            _dialogService.Close(nameof(LoadingDialogView));
            WeakReferenceMessenger.Default.Send<CustomObservableCollection<CodeInfo>, FolderCompareViewModel>(FileDatas);
        }        
    }
   
}
