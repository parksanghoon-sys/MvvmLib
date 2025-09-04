using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Components;
using CoreMvvmLib.Core.Services.DialogService;
using CoreMvvmLib.WPF.Components;
using Newtonsoft.Json;
using System.IO;
using System.Windows.Markup;
using wpfCodeCheck.Component.UI.Views;
using wpfCodeCheck.Domain.Enums;
using wpfCodeCheck.Domain.Helpers;
using wpfCodeCheck.Domain.Services;
using wpfCodeCheck.Domain.Services.Interfaces;
using wpfCodeCheck.ProjectChangeTracker.Local.Models;
using wpfCodeCheck.ProjectChangeTracker.Local.Services;

namespace wpfCodeCheck.ProjectChangeTracker.Local.ViewModels
{
    public partial class ComparisonResultsViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly ISettingService _settingService;
        private readonly IExcelProcessManager _excelProcessManager;
        private readonly IBaseService _baseService;
        private string _excelFilePath = string.Empty;
        public ComparisonResultsViewModel(IDialogService dialogService,
            ISettingService settingService,
            IExcelProcessManager excelProcessManager,
            IBaseService baseService)
        {
            _dialogService = dialogService;
            _settingService = settingService;
            _excelProcessManager = excelProcessManager;
            _baseService = baseService;

            ExportOutputPath = _settingService.GeneralSetting!.OutputExcelPath == string.Empty ? DirectoryHelper.GetLocalExportDirectory() : _settingService.GeneralSetting.OutputExcelPath;
            ExportOutputFileName = _settingService.GeneralSetting!.OutputExcelFileName == string.Empty ? "SW_Change" : _settingService.GeneralSetting.OutputExcelFileName;            
            DocumentNumber = "80541551SPS";
            Summery = "o 기능개선\r\n : 항전개조 1단계 적용 \r\n OT보완 개선사항 반영"; ;
        }

        [Property]
        private string _exportOutputPath = string.Empty;
        [Property]
        private string _exportOutputFileName = string.Empty;
        [Property]
        private EProjectType _projectType;
        [Property]
        private string _documentNumber = string.Empty;
        [Property]
        private string _summery = string.Empty;

        [Property]
        private CustomObservableCollection<FailClassAnalysisModel> _failFileDatas = new();
        [AsyncRelayCommand]
        private async Task ExportAsync()
        {
            if (_baseService.CompareResult is null)
                throw new NullReferenceException($"{nameof(_baseService.CompareResult)} 이 Null 입니다.");

            if (Directory.Exists(ExportOutputPath) == false)
                DirectoryHelper.CreateDirectory(ExportOutputPath);

            //string baseExcelFilepath = Path.Combine(Environment.CurrentDirectory, "SW_Chage.xlsx");
            _excelFilePath = Path.Combine(ExportOutputPath, ExportOutputFileName + ".xlsx");

            if (File.Exists(_excelFilePath) == true)
            {
                File.Delete(_excelFilePath);
            }

            //File.Copy(baseExcelFilepath, copyExcelFilePath);            

            _dialogService.Show(this, typeof(LoadingDialogView), 300, 300);
            var statementDto = new DetailedStatementDto
            {
                Summery = this.Summery,
                DocumentNumber = this.DocumentNumber,
                ProjectType = this.ProjectType,
            };
            statementDto = statementDto.CompareEntityToDetailedStatementDto(_baseService.CompareResult);

            var isSuccessed = await _excelProcessManager.RunAsync(statementDto, _excelFilePath);           

            _dialogService.Close(typeof(LoadingDialogView));

            _settingService.GeneralSetting!.OutputExcelPath = ExportOutputPath;
            _settingService.GeneralSetting!.OutputExcelFileName = ExportOutputFileName;
            _settingService.SaveSetting();
        }
        [AsyncRelayCommand]
        private async Task RetryAsync()
        {
            
        }
        [RelayCommand]
        private void FileOpen()
        {
            
        }
    }
}
