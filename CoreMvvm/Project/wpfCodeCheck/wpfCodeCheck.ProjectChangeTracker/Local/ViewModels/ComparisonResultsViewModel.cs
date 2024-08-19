using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Components;
using CoreMvvmLib.Core.Services.DialogService;
using System.IO;
using wpfCodeCheck.Component.UI.Views;
using wpfCodeCheck.Domain.Services;
using wpfCodeCheck.Domain.Helpers;
using wpfCodeCheck.Domain.Datas;

namespace wpfCodeCheck.ProjectChangeTracker.Local.ViewModels
{
    public partial class ComparisonResultsViewModel : ViewModelBase
    {
        private readonly IBaseService<CustomCodeComparer> _baseService;
        private readonly IDialogService _dialogService;
        private readonly ISettingService _settingService;
        private readonly IExcelPaser _excelPaser;
        private CodeDiffReulstModel<CustomCodeComparer> _diffModel;
        public ComparisonResultsViewModel(IBaseService<CustomCodeComparer> baseService, IDialogService dialogService, ISettingService settingService, IExcelPaser excelPaser)
        {
            _baseService = baseService;
            _dialogService = dialogService;
            _settingService = settingService;
            _excelPaser = excelPaser;
            ExportOutputPath = _settingService.GeneralSetting!.OutputExcelPath == string.Empty ? DirectoryHelper.GetLocalDirectory("EXPROT") : _settingService.GeneralSetting.OutputExcelPath;
            ExportOutputFileName = _settingService.GeneralSetting!.OutputExcelFileName == string.Empty ? "SW_Change" : _settingService.GeneralSetting.OutputExcelFileName;
            //WeakReferenceMessenger.Default.Register<ComparisonResultsViewModel, CodeDiffReulstModel>(this, OnReceiveCodeInfos);
        }

        private void OnReceiveCodeInfos(ComparisonResultsViewModel model1, CodeDiffReulstModel<CustomCodeComparer> model2)
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
            //string baseExcelFilepath = Path.Combine(Environment.CurrentDirectory, "SW_Chage.xlsx");
            string copyExcelFilePath = Path.Combine(ExportOutputPath , ExportOutputFileName +".xlsx");


            if (File.Exists(copyExcelFilePath) == true)
            {
                File.Delete(copyExcelFilePath);
            }

            //File.Copy(baseExcelFilepath, copyExcelFilePath);

            _excelPaser.SetFilePath(copyExcelFilePath);
            _excelPaser.SetExcelData(_baseService.CompareResult);

            _dialogService.Show(this, typeof(LoadingDialogView), 300, 300);
            
            await _excelPaser.WriteExcelAync();

            _dialogService.Close(typeof(LoadingDialogView));
            _settingService.GeneralSetting!.OutputExcelPath = ExportOutputPath;
            _settingService.GeneralSetting!.OutputExcelFileName = ExportOutputFileName;
            _settingService.SaveSetting();
        }       
    }
}
