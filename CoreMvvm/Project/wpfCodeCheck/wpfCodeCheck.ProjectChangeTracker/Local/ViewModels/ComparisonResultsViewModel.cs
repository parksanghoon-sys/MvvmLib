using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Components;
using CoreMvvmLib.Core.Services.DialogService;
using System.IO;
using wpfCodeCheck.Component.UI.Views;
using wpfCodeCheck.Domain.Services;
using wpfCodeCheck.Domain.Helpers;
using Newtonsoft.Json;
using wpfCodeCheck.ProjectChangeTracker.Local.Models;
using CoreMvvmLib.WPF.Components;

namespace wpfCodeCheck.ProjectChangeTracker.Local.ViewModels
{
    public partial class ComparisonResultsViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly ISettingService _settingService;
        private readonly IExcelPaser _excelPaser;
        private readonly IBaseService _baseService;
        private string _excelFilePath = string.Empty;
        public ComparisonResultsViewModel(IDialogService dialogService, 
            ISettingService settingService, 
            IExcelPaser excelPaser,
            IBaseService baseService)
        {
            _dialogService = dialogService;
            _settingService = settingService;
            _excelPaser = excelPaser;
            _baseService = baseService;

            ExportOutputPath = _settingService.GeneralSetting!.OutputExcelPath == string.Empty ? DirectoryHelper.GetLocalExportDirectory() : _settingService.GeneralSetting.OutputExcelPath;
            ExportOutputFileName = _settingService.GeneralSetting!.OutputExcelFileName == string.Empty ? "SW_Change" : _settingService.GeneralSetting.OutputExcelFileName;            
        }

        //private void OnReceiveCodeInfos(ComparisonResultsViewModel model1, CodeDiffReulstModel<CustomCodeComparer> model2)
        //{
        //    if(model2 is not null)
        //    {
        //        _diffModel = model2;
        //    }
        //}

        [Property]
        private string _exportOutputPath = string.Empty;
        [Property]
        private string _exportOutputFileName = string.Empty;
        [Property]
        private CustomObservableCollection<FailClassAnalysisModel> _failFileDatas = new();
        [AsyncRelayCommand]
        private async Task ExportAsync()
        {
            DirectoryHelper.CreateDirectory(ExportOutputPath);
            string Excel_DATA_PATH = ExportOutputPath;
            //string baseExcelFilepath = Path.Combine(Environment.CurrentDirectory, "SW_Chage.xlsx");
            _excelFilePath = Path.Combine(ExportOutputPath, ExportOutputFileName + ".xlsx");


            if (File.Exists(_excelFilePath) == true)
            {
                File.Delete(_excelFilePath);
            }

            //File.Copy(baseExcelFilepath, copyExcelFilePath);            

            _dialogService.Show(this, typeof(LoadingDialogView), 300, 300);

            var isAllSuccess = await _excelPaser.WriteExcelAync(_excelFilePath);
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

            File.Delete(jsonFilePath);
            _dialogService.Show(this, typeof(LoadingDialogView), 300, 300);

            foreach (var failFile in _failFileDatas)
            {
                var isSuccess = await _excelPaser.WriteExcelAync(failFile.InputFile, failFile.OutputFile);
            }
            FailFileDatas.Clear();
            //if (File.Exists(jsonFilePath))
            //{
            //    var jsonStr = File.ReadAllText(jsonFilePath);

            //    FailFileDatas.AddRange(JsonConvert.DeserializeObject<List<FailClassAnalysisModel>>(jsonStr));
            //}
            _dialogService.Close(typeof(LoadingDialogView));
        }
        [RelayCommand]
        private void FileOpen()
        {

        }
    }
}
