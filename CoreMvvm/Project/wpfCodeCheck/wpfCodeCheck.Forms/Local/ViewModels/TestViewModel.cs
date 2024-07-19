using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Components;
using System.Windows;
using wpfCodeCheck.Domain.Services;
using wpfCodeCheck.Main.UI.Units;

namespace wpfCodeCheck.Forms.Local.ViewModels
{
    public partial class TestViewModel : ViewModelBase
    {
        private readonly IFileCheckSum _fileCheckSum;
        

        public TestViewModel(IFileCheckSum fileCheckSum)
        {
            _fileCheckSum = fileCheckSum;
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
