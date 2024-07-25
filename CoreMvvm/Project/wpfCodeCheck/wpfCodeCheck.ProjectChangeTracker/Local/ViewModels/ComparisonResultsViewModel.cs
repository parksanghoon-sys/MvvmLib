using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Components;
using CoreMvvmLib.Core.Services.DialogService;
using System.IO;
using wpfCodeCheck.ProjectChangeTracker.Local.Services;
using wpfCodeCheck.Component.UI.Views;
using wpfCodeCheck.Domain.Services;
using wpfCodeCheck.Domain.Helpers;
using wpfCodeCheck.Domain.Datas;

namespace wpfCodeCheck.ProjectChangeTracker.Local.ViewModels
{
    public partial class ComparisonResultsViewModel : ViewModelBase
    {
        private readonly IBaseService _baseService;
        private readonly IDialogService _dialogService;
        private readonly ISettingService _settingService;
        private CodeDiffModel _diffModel;
        public ComparisonResultsViewModel(IBaseService baseService, IDialogService dialogService, ISettingService settingService)
        {
            _baseService = baseService;
            _dialogService = dialogService;
            _settingService = settingService;            
            ExportOutputPath = _settingService.GeneralSetting!.OutputExcelPath == string.Empty ? DirectoryHelper.GetLocalDirectory("EXPROT") : _settingService.GeneralSetting.OutputExcelPath;
            ExportOutputFileName = _settingService.GeneralSetting!.OutputExcelFileName == string.Empty ? "SW_Change" : _settingService.GeneralSetting.OutputExcelFileName;
            //WeakReferenceMessenger.Default.Register<ComparisonResultsViewModel, CodeDiffModel>(this, OnReceiveCodeInfos);
        }

        private void OnReceiveCodeInfos(ComparisonResultsViewModel model1, CodeDiffModel model2)
        {
            if(model2 is not null)
            {
                _diffModel = model2;
            }
        }

        [Property]
        private string _exportOutputPath =string.Empty;
        [Property]
        private string _exportOutputFileName = string.Empty;
        [AsyncRelayCommand]
        private async Task ExportAsync()
        {
            DirectoryHelper.CreateDirectory(ExportOutputPath);
            string Excel_DATA_PATH = ExportOutputPath;
            string baseExcelFilepath = Path.Combine(Environment.CurrentDirectory, "SW_Chage.xlsx");
            string copyExcelFilePath = Path.Combine(ExportOutputPath , ExportOutputFileName +".xlsx");
            

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
            _settingService.GeneralSetting!.OutputExcelPath = ExportOutputPath;
            _settingService.GeneralSetting!.OutputExcelFileName = ExportOutputFileName;
            _settingService.SaveSetting();
        }       
    }
}
