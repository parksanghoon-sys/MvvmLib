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
using CoreMvvmLib.WPF.Components;

namespace wpfCodeCheck.ProjectChangeTracker.Local.ViewModels
{
    public partial class ComparisonResultsViewModel : ViewModelBase
    {        
        private readonly IDialogService _dialogService;
        private readonly ISettingService _settingService;
        private readonly IExcelPaser _excelPaser;
        private string _excelFilePath = string.Empty;
        public ComparisonResultsViewModel(IDialogService dialogService, ISettingService settingService, IExcelPaser excelPaser)
        {            
            _dialogService = dialogService;
            _settingService = settingService;
            _excelPaser = excelPaser;
            
            ExportOutputPath = _settingService.GeneralSetting!.OutputExcelPath == string.Empty ? DirectoryHelper.GetLocalDirectory("EXPROT") : _settingService.GeneralSetting.OutputExcelPath;
            ExportOutputFileName = _settingService.GeneralSetting!.OutputExcelFileName == string.Empty ? "SW_Change" : _settingService.GeneralSetting.OutputExcelFileName;

            //string jsonFilePath = Path.Combine(ExportOutputPath, ExportOutputFileName);
            //jsonFilePath += ".json";
            //var jsonStr = File.ReadAllText(jsonFilePath);
            //FailFileDatas.AddRange(JsonConvert.DeserializeObject<List<FailClassAnalysisModel>>(jsonStr));
        }
        [Property]
        private string _exportOutputPath =string.Empty;
        [Property]
        private string _exportOutputFileName = string.Empty;
        [Property]
        private CustomObservableCollection<FailClassAnalysisModel> _failFileDatas = new();
        [Property]
        private FailClassAnalysisModel _failFileData = new();
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
            _excelFilePath = Path.Combine(ExportOutputPath , ExportOutputFileName +".xlsx");


            if (File.Exists(_excelFilePath) == true)
            {
                File.Delete(_excelFilePath);
            }

            //File.Copy(baseExcelFilepath, copyExcelFilePath);

            _excelPaser.SetFilePath(_excelFilePath);            

            _dialogService.Show(this, typeof(LoadingDialogView), 300, 300);
            
            var isAllSuccess = await _excelPaser.WriteExcelAync();
            if (isAllSuccess == false)
            {
                string jsonFilePath = _excelFilePath.Replace(".xlsx", ".json");
                var jsonStr = File.ReadAllText(jsonFilePath);
                FailFileDatas.AddRange(JsonConvert.DeserializeObject<List<FailClassAnalysisModel>>(jsonStr));
                
            }

            _dialogService.Close(typeof(LoadingDialogView));

            _settingService.GeneralSetting!.OutputExcelPath = ExportOutputPath;
            _settingService.GeneralSetting!.OutputExcelFileName = ExportOutputFileName;
            _settingService.SaveSetting();
        }
        [AsyncRelayCommand]
        private async Task RetryAsync()
        {
            string jsonFilePath = _excelFilePath.Replace(".xlsx", ".json");

            FailFileDatas.Clear();

            File.Delete(jsonFilePath);
            foreach (var failFile in _failFileDatas)
            {
                var isSuccess = await _excelPaser.WriteExcelAync(failFile.InputFile, failFile.OutputFile);
            }            
            if (File.Exists(jsonFilePath))
            {
                var jsonStr = File.ReadAllText(jsonFilePath);
                
                FailFileDatas.AddRange(JsonConvert.DeserializeObject<List<FailClassAnalysisModel>>(jsonStr));
            }                        
        }
    }
}
