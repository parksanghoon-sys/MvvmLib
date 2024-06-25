using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Components;
using System.Windows;
using wpfCodeCheck.Main.UI.Units;

namespace wpfCodeCheck.Main.Local.ViewModels
{
    public partial class FolderListViewModel : ViewModelBase
    {
        public FolderListViewModel()
        {
            
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
