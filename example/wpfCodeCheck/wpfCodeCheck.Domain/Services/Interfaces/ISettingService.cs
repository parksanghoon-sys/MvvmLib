using wpfCodeCheck.Domain.UseCases.Settings;

namespace wwpfCodeCheck.Domain.Services.Interfaces;
public interface ISettingService
{        
    GeneralSetting? GeneralSetting { get; }
    WindowSetting? WindowSetting { get; }
    void SaveSetting();
}

