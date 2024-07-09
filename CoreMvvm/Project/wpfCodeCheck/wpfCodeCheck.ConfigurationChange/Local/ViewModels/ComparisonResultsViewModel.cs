using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Components;
using System.IO;
using System.Windows;
using wpfCodeCheck.ConfigurationChange.Local.Services;
using wpfCodeCheck.Shared.Local.Services;

namespace wpfCodeCheck.ConfigurationChange.Local.ViewModels
{
    public partial class ComparisonResultsViewModel : ViewModelBase
    {
        private readonly IBaseService _baseService;

        public ComparisonResultsViewModel(IBaseService baseService)
        {
            _baseService = baseService;
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
            await excelPaser.WriteExcel();
            MessageBox.Show("완료");
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
