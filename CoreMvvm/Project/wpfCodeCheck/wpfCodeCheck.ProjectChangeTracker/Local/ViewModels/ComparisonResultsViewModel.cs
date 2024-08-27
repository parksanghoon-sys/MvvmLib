using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Components;
using CoreMvvmLib.Core.Services.DialogService;
using System.IO;
using wpfCodeCheck.Component.UI.Views;
using wpfCodeCheck.Domain.Services;
using wpfCodeCheck.Domain.Helpers;
using wpfCodeCheck.Domain.Datas;
using Newtonsoft.Json;
using wpfCodeCheck.ProjectChangeTracker.Local.Models;
using System.Diagnostics;

namespace wpfCodeCheck.ProjectChangeTracker.Local.ViewModels
{
    public partial class ComparisonResultsViewModel : ViewModelBase
    {        
        private readonly IDialogService _dialogService;
        private readonly ISettingService _settingService;
        private readonly IExcelPaser _excelPaser;
        public ComparisonResultsViewModel(IDialogService dialogService, ISettingService settingService, IExcelPaser excelPaser)
        {            
            _dialogService = dialogService;
            _settingService = settingService;
            _excelPaser = excelPaser;
            
            ExportOutputPath = _settingService.GeneralSetting!.OutputExcelPath == string.Empty ? DirectoryHelper.GetLocalDirectory("EXPROT") : _settingService.GeneralSetting.OutputExcelPath;
            ExportOutputFileName = _settingService.GeneralSetting!.OutputExcelFileName == string.Empty ? "SW_Change" : _settingService.GeneralSetting.OutputExcelFileName;
        }
        [Property]
        private string _exportOutputPath =string.Empty;
        [Property]
        private string _exportOutputFileName = string.Empty;
        [RelayCommand]
        private void FileOpen()
        {
            string filePath = Path.Combine(ExportOutputPath, $"{ExportOutputFileName}.xlsx");
            //File.Open(filePath, FileMode.Open);
            Process.Start(filePath);
        }
        [AsyncRelayCommand]
        private async Task ExportAsync()
        {
            DirectoryHelper.CreateDirectory(ExportOutputPath);
            string Excel_DATA_PATH = ExportOutputPath;
            //string baseExcelFilepath = Path.Combine(Environment.CurrentDirectory, "SW_Chage.xlsx");
            string copyExcelFilePath = Path.Combine(ExportOutputPath , ExportOutputFileName +".xlsx");


            if (File.Exists(copyExcelFilePath) == true)
            {
                File.Delete(copyExcelFilePath);
            }

            //File.Copy(baseExcelFilepath, copyExcelFilePath);

            _excelPaser.SetFilePath(copyExcelFilePath);            

            _dialogService.Show(this, typeof(LoadingDialogView), 300, 300);
            
            var isAllSuccess = await _excelPaser.WriteExcelAync();
            if (isAllSuccess == false)
            {
                string jsonFilePath = copyExcelFilePath.Replace(".xlsx", ".json");
                var jsonStr = File.ReadAllText(jsonFilePath);
                List<FailClassAnalysisModel> people = JsonConvert.DeserializeObject<List<FailClassAnalysisModel>>(jsonStr);
            }

            _dialogService.Close(typeof(LoadingDialogView));

            _settingService.GeneralSetting!.OutputExcelPath = ExportOutputPath;
            _settingService.GeneralSetting!.OutputExcelFileName = ExportOutputFileName;
            _settingService.SaveSetting();


        }       
    }
}
