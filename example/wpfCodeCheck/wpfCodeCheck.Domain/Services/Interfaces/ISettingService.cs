using wpfCodeCheck.Domain.UseCases.Settings;

namespace wpfCodeCheck.Domain.Services.Interfaces;
public interface ISettingService
{        
    GeneralSetting? GeneralSetting { get; }
    WindowSetting? WindowSetting { get; }
    void SaveSetting();
}

