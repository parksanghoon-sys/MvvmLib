using wpfCodeCheck.Domain.UseCases.Settings;

namespace wpfCodeCheck.Domain.Services
{
    public interface ISettingService
    {
        string UseAppDataPath { get; set; }
        GeneralSetting? GeneralSetting { get; }
        WindowSetting? WindowSetting { get; }
        void SaveSetting();
    }
}
