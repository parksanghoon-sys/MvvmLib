using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Components;
using System.Windows;
using wpfCodeCheck.Component.UI.Units;
using wpfCodeCheck.Domain.Services.Interfaces;

namespace wpfCodeCheck.Forms.Local.ViewModels
{
    public partial class TestViewModel : ViewModelBase
    {
        private readonly IFileCheckSum _fileCheckSum;
        [Property]
        private string _export = string.Empty;

        public TestViewModel(IFileCheckSum fileCheckSum)
        {
            _fileCheckSum = fileCheckSum;
            Export = "Test";
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
