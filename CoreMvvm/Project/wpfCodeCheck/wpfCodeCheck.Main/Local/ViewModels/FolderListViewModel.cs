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

namespace wpfCodeCheck.Main.Local.ViewModels
{
    public partial class FolderListViewModel : ViewModelBase
    {
        private readonly IFileCheckSum _fileCheckSum;
        private readonly IDierctoryFileInfoService _dierctoryFileInfoService;

        public FolderListViewModel(IFileCheckSum fileCheckSum, IDierctoryFileInfoService dierctoryFileInfoService)
        {
            _fileCheckSum = fileCheckSum;
            _dierctoryFileInfoService = dierctoryFileInfoService;
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
            dlg.InitialExpandedFolder = @"c:\";
            dlg.OKButtonText = "OK!";
            if (true == dlg.ShowDialog())
            {
                _fileDatas.Clear();
                FolderPath = dlg.SelectedFolder;
                var folderInfoList = await _dierctoryFileInfoService.GetDirectoryCodeFileInfosAsync(FolderPath);
                _fileDatas.AddRange(folderInfoList);
                //MessageBox.Show(dlg.SelectedFolder, "Selected Folder");
            }

        }
    }
   
}
