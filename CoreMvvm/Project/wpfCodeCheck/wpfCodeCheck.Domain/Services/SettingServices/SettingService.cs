﻿using CoreMvvmLib.Core.Helpers;
using System.Reflection;
using wpfCodeCheck.Domain.Helpers;
using wpfCodeCheck.Domain.UseCases.Settings;

namespace wpfCodeCheck.Domain.Services
{
    public class SettingService : ISettingService
    {
        private GeneralSetting? _generalSetting;
        private WindowSetting? _windowSetting;        
        public GeneralSetting? GeneralSetting => _generalSetting;

        public WindowSetting? WindowSetting => _windowSetting;        

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
        public void SaveSetting()
        {
            SettingsProvider settingsProvider = new SettingsProvider();
            settingsProvider.generalSetting = GeneralSetting;
            settingsProvider.windowSetting = WindowSetting;

            SerializeHelper.SaveDataToXml<SettingsProvider>(DirectoryHelper.GetLocalSettingDirectory("Settings.xml"), settingsProvider, true);
        }
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
