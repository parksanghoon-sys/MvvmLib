using wpfCodeCheck.Domain.UseCases.Settings;

namespace wpfCodeCheck.Domain.Services
{
    public interface ISettingService
    {        
        GeneralSetting? GeneralSetting { get; }
        WindowSetting? WindowSetting { get; }
        void SaveSetting();
    }
}
