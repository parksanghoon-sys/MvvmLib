using wpfCodeCheck.Domain.UseCases.Settings;

namespace wpfCodeCheck.Domain.Services.Interfaces;

/// <summary>
/// 애플리케이션 설정 관리 기능을 정의하는 인터페이스
/// 일반 설정과 창 설정을 로드하고 저장하는 기능을 제공합니다.
/// </summary>
public interface ISettingService
{        
    /// <summary>
    /// 일반 설정을 가져옵니다.
    /// </summary>
    GeneralSetting? GeneralSetting { get; }
    
    /// <summary>
    /// 창 설정을 가져옵니다.
    /// </summary>
    WindowSetting? WindowSetting { get; }
    /// <summary>
    /// 현재 설정을 파일로 저장합니다.
    /// </summary>
    void SaveSetting();
}

