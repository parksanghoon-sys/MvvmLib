using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Components;
using CoreMvvmLib.Core.Services.DialogService;
using System.IO;
using System.Windows;
using wpfCodeCheck.ConfigurationChange.Local.Services;
using wpfCodeCheck.Shared.Local.Services;
using wpfCodeCheck.Shared.UI.Views;

namespace wpfCodeCheck.ConfigurationChange.Local.ViewModels
{
    public partial class ComparisonResultsViewModel : ViewModelBase
    {
        private readonly IBaseService _baseService;
        private readonly IDialogService _dialogService;

        public ComparisonResultsViewModel(IBaseService baseService, IDialogService dialogService)
        {
            _baseService = baseService;
            _dialogService = dialogService;
        }
        [AsyncRelayCommand]
        private async Task AsyncExport()
        {
            string Excel_DATA_PATH = Path.Combine(Environment.CurrentDirectory, @"ExportExcel\");
            string baseExcelFilepath = Path.Combine(Environment.CurrentDirectory, "SW_Chage.xlsx");
            string copyExcelFilePath = Excel_DATA_PATH + Path.GetFileName("SW_Chage_Test.xlsx");

            CreatePathFolder(Excel_DATA_PATH);
            if (File.Exists(copyExcelFilePath) == true)
            {
                File.Delete(copyExcelFilePath);
            }

            File.Copy(baseExcelFilepath, copyExcelFilePath);

            IExcelPaser excelPaser = new ClosedXmlUsedExcelParser(copyExcelFilePath);
            excelPaser.SetExcelDate(_baseService.CompareResult);

            _dialogService.Show(this, nameof(LoadingDialogView), 300, 300);
            

            await excelPaser.WriteExcel();
            _dialogService.Close(nameof(LoadingDialogView));
        }
        private void CreatePathFolder(string path)
        {
            string[] folderNames = path.Split('\\');
            string fullPath = string.Empty;
            for (int i = 0; i < folderNames.Length - 1; i++)
            {
                fullPath += folderNames[i] + '\\';
                DirectoryInfo di = new DirectoryInfo(fullPath);
                if (!di.Exists) di.Create();
            }
        }
    }
}
