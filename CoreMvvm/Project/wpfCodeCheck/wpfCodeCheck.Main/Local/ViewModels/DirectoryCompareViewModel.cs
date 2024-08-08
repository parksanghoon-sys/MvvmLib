using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Components;
using CoreMvvmLib.Core.Messenger;
using wpfCodeCheck.Domain.Enums;
using wpfCodeCheck.Domain.Services;
using wpfCodeCheck.Main.UI.Units;

namespace wpfCodeCheck.Main.Local.ViewModels
{
    public partial class DirectoryCompareViewModel : ViewModelBase
    {
        private readonly ISettingService _settingService;

        public DirectoryCompareViewModel(ISettingService settingService)
        {
            _settingService = settingService;
            InputDirectoryPath = _settingService.GeneralSetting!.InputPath ?? "";
            OutputDirectoryPath = _settingService.GeneralSetting!.OutputPath ?? "";
        }
        [Property]
        private string _inputDirectoryPath = string.Empty;
        [Property]
        private string _outputDirectoryPath = string.Empty;
        [RelayCommand]
        private void Compare()
        {
            _settingService.GeneralSetting!.InputPath = InputDirectoryPath;
            _settingService.GeneralSetting!.OutputPath= OutputDirectoryPath;

            _settingService.SaveSetting();
            WeakReferenceMessenger.Default.Send<EMainViewType>(EMainViewType.FILE_CHECKSUM);
        }

        //[RelayCommand]
        //private void FileDialogOpen(string type)
        //{
        //    BrowseForFolderDialog dlg = new BrowseForFolderDialog();
        //    dlg.Title = "Select a folder and click OK!";
        //    dlg.InitialExpandedFolder = @"D:\Project\01.Program\2023\GcsProject\2.FlightSolution";
        //    dlg.OKButtonText = "OK!";
        //    if (true == dlg.ShowDialog())
        //    {
        //        if(string.Equals(type,"Input"))
        //            InputDirectoryPath = dlg.SelectedFolder;
        //        else
        //            OutputDirectoryPath = dlg?.SelectedFolder;
        //    }
        //}
    }
}
