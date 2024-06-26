using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Components;
using System.Windows;
using wpfCodeCheck.Main.Local.Servies.DirectoryService;
using wpfCodeCheck.Main.Local.Servies;
using wpfCodeCheck.Main.UI.Units;
using System.Collections.ObjectModel;
using System.IO;
using wpfCodeCheck.Main.Local.Models;
using System.Collections.Specialized;
using CoreMvvmLib.WPF.Components;
using CoreMvvmLib.Core.Services.DialogService;
using wpfCodeCheck.Sub.UI.Views;
using CoreMvvmLib.Core.Messenger;
using CoreMvvmLib.Core.IOC;

namespace wpfCodeCheck.Main.Local.ViewModels
{
    public partial class FolderListViewModel : ViewModelBase
    {
        private readonly IDierctoryFileInfoService _dierctoryFileInfoService;
        private readonly IDialogService _dialogService;

        public FolderListViewModel( IDierctoryFileInfoService dierctoryFileInfoService, IDialogService dialogService)
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

        [RelayCommand]
        public async void FileDialogOpen()
        {
            BrowseForFolderDialog dlg = new BrowseForFolderDialog();
            dlg.Title = "Select a folder and click OK!";
            dlg.InitialExpandedFolder = @"D:\Project\01.Program\2023\GcsProject\2.FlightSolution";
            dlg.OKButtonText = "OK!";
            if (true == dlg.ShowDialog())
            {                
                _dialogService.Show(this, nameof(LoadingDialogView), 300, 300);
                WeakReferenceMessenger.Default.Send<bool>(true);

                _fileDatas.Clear();
                FolderPath = dlg.SelectedFolder;

                var folderInfoList = await _dierctoryFileInfoService.GetDirectoryCodeFileInfosAsync(FolderPath);
                _fileDatas.AddRange(folderInfoList);
                
            }
            _dialogService.Close(nameof(LoadingDialogView));
            WeakReferenceMessenger.Default.Send<bool>(false);
        }
    }
   
}
