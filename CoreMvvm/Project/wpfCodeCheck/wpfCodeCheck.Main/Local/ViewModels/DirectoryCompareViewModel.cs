using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Components;
using CoreMvvmLib.Core.Messenger;
using wpfCodeCheck.Domain.Enums;
using wpfCodeCheck.Domain.Services;

namespace wpfCodeCheck.Main.Local.ViewModels
{
    /// <summary>
    /// 디렉토리 비교 ListView 뷰모델
    /// </summary>
    public partial class DirectoryCompareViewModel : ViewModelBase
    {
        private readonly ISettingService _settingService;

        public DirectoryCompareViewModel(ISettingService settingService)
        {
            _settingService = settingService;
            InputDirectoryPath = _settingService.GeneralSetting!.InputPath ?? "";
            OutputDirectoryPath = _settingService.GeneralSetting!.OutputPath ?? "";
            InputType = _settingService.GeneralSetting!.CompareType;
        }
        [Property]
        private string _inputDirectoryPath = string.Empty;
        [Property]
        private string _outputDirectoryPath = string.Empty;        
        private EType _inputType;
        public EType InputType
        {
            get => _inputType;
            set
            {
                _inputType = value;
                SetProperty(ref _inputType, value);
                OnPropertyChanged(nameof(IsEnbaleComapre));
            }
        }    
        public bool IsEnbaleComapre  => InputType != EType.NONE;

        [RelayCommand]
        private void Compare()
        {
            _settingService.GeneralSetting!.InputPath = InputDirectoryPath;
            _settingService.GeneralSetting!.OutputPath= OutputDirectoryPath;
            _settingService.GeneralSetting.CompareType = InputType;

            _settingService.SaveSetting();
            WeakReferenceMessenger.Default.Send<EMainViewType>(EMainViewType.FILE_CHECKSUM);
        }    
    }
}
