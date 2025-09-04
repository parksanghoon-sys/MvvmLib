using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Components;
using CoreMvvmLib.Core.Messenger;
using System.IO;
using System.Windows;
using wpfCodeCheck.Domain.Enums;
using wpfCodeCheck.Domain.Services;
using wpfCodeCheck.Domain.Services.Interfaces;
using wpfCodeCheck.Domain.Services.LogService;

namespace wpfCodeCheck.Main.Local.ViewModels
{
    /// <summary>
    /// 디렉토리 비교 ListView 뷰모델
    /// </summary>
    public partial class DirectoryCompareViewModel : ViewModelBase
    {
        private readonly ISettingService _settingService;        
        private readonly ILoggerService _loggerService;

        public DirectoryCompareViewModel(ISettingService settingService,             
            ILoggerService loggerService)
        {
            _settingService = settingService;            
            _loggerService = loggerService;
            InputDirectoryPath = _settingService.GeneralSetting!.InputPath ?? "";
            OutputDirectoryPath = _settingService.GeneralSetting!.OutputPath ?? "";
            InputType = _settingService.GeneralSetting!.CompareType;
        }
        [Property]
        private string _inputDirectoryPath = string.Empty;
        [Property]
        private string _outputDirectoryPath = string.Empty;        
        private ECompareType _inputType;
        public ECompareType InputType
        {
            get => _inputType;
            set
            {
                _inputType = value;
                SetProperty(ref _inputType, value);
                OnPropertyChanged(nameof(IsEnbaleComapre));
            }
        }    
        public bool IsEnbaleComapre  => InputType != ECompareType.NONE;

        [RelayCommand]
        private void Compare()
        {
            if (!Directory.Exists(InputDirectoryPath))
            {
                MessageBox.Show("InputDirectory 경로를 입력하세요");
                return;
            }
            if (!Directory.Exists(OutputDirectoryPath))
            {
                MessageBox.Show("OutputDirectory 경로를 입력하세요");
                return;
            }
            _settingService.GeneralSetting!.InputPath = InputDirectoryPath;
            _settingService.GeneralSetting!.OutputPath= OutputDirectoryPath;
            _settingService.GeneralSetting.CompareType = InputType;
            _loggerService.Info($"START : InputPath : {InputDirectoryPath}, OutputPath : {OutputDirectoryPath}");
            _settingService.SaveSetting();
            
            WeakReferenceMessenger.Default.Send<EMainViewType>(EMainViewType.FILE_CHECKSUM);
        }    
    }
}
