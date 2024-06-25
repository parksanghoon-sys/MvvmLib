using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Commands;
using CoreMvvmLib.Core.Components;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using wpfCodeCheck.Main.Local.Servies;
using wpfCodeCheck.Main.Local.Servies.DirectoryService;
using wpfCodeCheck.Main.UI.Units;

namespace wpfCodeCheck.Forms.Local.ViewModels
{
    public partial class TestViewModel : ViewModelBase
    {
        private readonly IFileCheckSum _fileCheckSum;
        private readonly IDierctoryFileInfoService _dierctoryFileInfoService;

        public TestViewModel(IFileCheckSum fileCheckSum, IDierctoryFileInfoService dierctoryFileInfoService)
        {
            _fileCheckSum = fileCheckSum;
            _dierctoryFileInfoService = dierctoryFileInfoService;
        }        
        [RelayCommand]
        public void FileDialogOpen()
        {
            BrowseForFolderDialog dlg = new BrowseForFolderDialog();
            dlg.Title = "Select a folder and click OK!";
            dlg.InitialExpandedFolder = @"c:\";
            dlg.OKButtonText = "OK!";
            if (true == dlg.ShowDialog())
            {
                MessageBox.Show(dlg.SelectedFolder, "Selected Folder");
            }
        }
    }
}
