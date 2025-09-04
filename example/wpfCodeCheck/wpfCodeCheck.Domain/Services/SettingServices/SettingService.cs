using CoreMvvmLib.Core.Helpers;
using System.Reflection;
using wpfCodeCheck.Domain.Helpers;
using wpfCodeCheck.Domain.UseCases.Settings;
using wpfCodeCheck.Domain.Services.Interfaces;

namespace wpfCodeCheck.Domain.Services
{
    /// <summary>
    /// 애플리케이션의 설정 데이터를 관리하는 서비스
    /// XML 파일로부터 설정을 로드하고 저장하는 기능을 제공합니다.
    /// </summary>
    public class SettingService : ISettingService
    {
        /// <summary>
        /// 일반 설정 데이터
        /// </summary>
        private GeneralSetting? _generalSetting;
        /// <summary>
        /// 창 설정 데이터
        /// </summary>
        private WindowSetting? _windowSetting;        
        /// <summary>
        /// 일반 설정을 가져옵니다.
        /// </summary>
        public GeneralSetting? GeneralSetting => _generalSetting;

        /// <summary>
        /// 창 설정을 가져옵니다.
        /// </summary>
        public WindowSetting? WindowSetting => _windowSetting;        

        /// <summary>
        /// SettingService 생성자
        /// 설정 파일을 로드하거나 기본 설정을 생성합니다.
        /// </summary>
        public SettingService()
        {
            var settingFilePath = DirectoryHelper.GetLocalSettingDirectory("Settings.xml");
            if (File.Exists(settingFilePath))
            {
                SettingsProvider settingsProvider = SerializeHelper.ReadDataFromXmlFIle<SettingsProvider>(DirectoryHelper.GetLocalSettingDirectory("Settings.xml"), true);
                _generalSetting = settingsProvider.generalSetting ?? new GeneralSetting();
                _windowSetting = settingsProvider.windowSetting ?? new WindowSetting();
            }
            else
            {
                _generalSetting = new GeneralSetting();
                _windowSetting = new WindowSetting();
                SaveSetting();
            }
            SetDefaultValue();
        }
        /// <summary>
        /// 현재 설정을 XML 파일로 저장합니다.
        /// </summary>
        public void SaveSetting()
        {
            SettingsProvider settingsProvider = new SettingsProvider();
            settingsProvider.generalSetting = GeneralSetting;
            settingsProvider.windowSetting = WindowSetting;

            SerializeHelper.SaveDataToXml<SettingsProvider>(DirectoryHelper.GetLocalSettingDirectory("Settings.xml"), settingsProvider, true);
        }
        /// <summary>
        /// SettingAttribute를 통해 정의된 기본값을 설정합니다.
        /// null이거나 0인 속성들에 대해 기본값을 설정합니다.
        /// </summary>
        private void SetDefaultValue()
        {
            var propertyInfoArr = _generalSetting.GetType().GetProperties();
            {
                foreach (PropertyInfo pi in propertyInfoArr)
                {
                    if (pi.GetValue(_generalSetting) == null ||
                        (pi.GetValue(_generalSetting) is int && ((int)pi.GetValue(_generalSetting)) == 0) ||
                        (pi.GetValue(_generalSetting) is double && ((double)pi.GetValue(_generalSetting)) == 0) ||
                        (pi.GetValue(_generalSetting) is float && ((float)pi.GetValue(_generalSetting)) == 0))
                    {
                        SettingAttribute settingAtt = pi.GetCustomAttribute<SettingAttribute>();
                        pi.SetValue(_generalSetting, settingAtt.DefaultValue);
                    }
                }
                propertyInfoArr = _windowSetting.GetType().GetProperties();
                foreach (PropertyInfo pi in propertyInfoArr)
                {
                    if (pi.GetValue(_windowSetting) == null ||
                        (pi.GetValue(_windowSetting) is int && ((int)pi.GetValue(_windowSetting)) == 0) ||
                        (pi.GetValue(_windowSetting) is double && ((double)pi.GetValue(_windowSetting)) == 0) ||
                        (pi.GetValue(_windowSetting) is float && ((float)pi.GetValue(_windowSetting)) == 0))
                    {
                        SettingAttribute settingAtt = pi.GetCustomAttribute<SettingAttribute>();
                        pi.SetValue(_windowSetting, settingAtt.DefaultValue);
                    }
                }
            }

        }
    }
}
