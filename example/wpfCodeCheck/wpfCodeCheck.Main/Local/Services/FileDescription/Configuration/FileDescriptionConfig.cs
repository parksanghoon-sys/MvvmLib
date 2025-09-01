using wpfCodeCheck.Main.Local.Services.FileDescription.Models;

namespace wpfCodeCheck.Main.Local.Services.FileDescription.Configuration
{
    /// <summary>
    /// 파일 설명 규칙 설정 클래스
    /// 설정 파일이나 DB에서 로드할 수 있도록 분리
    /// </summary>
    public static class FileDescriptionConfig
    {
        /// <summary>
        /// 기본 파일 설명 규칙들
        /// </summary>
        public static List<FileDescriptionRule> GetDefaultRules()
        {
            return new List<FileDescriptionRule>
            {
                // === 경로 기반 규칙 (높은 우선순위) ===
                // Settings 폴더 관련
                new FileDescriptionRule
                {
                    Name = "Settings_Config",
                    PathPattern = "*Settings*Config*",
                    Description = "실행 파일 관련 설정 파일",
                    Category = FileCategory.Configuration,
                    Priority = 10
                },
                
                new FileDescriptionRule
                {
                    Name = "Settings_AutoSatProfile",
                    PathPattern = "*Settings*Csv*AutoSatProfile*",
                    Description = "위성 프로파일",
                    Category = FileCategory.Data,
                    Priority = 5
                },
                
                new FileDescriptionRule
                {
                    Name = "Settings_FlightControl_Status",
                    PathPattern = "*Settings*Csv*StatusCsv*",
                    Description = "비행통제 정보 상세보기 파일",
                    Category = FileCategory.Data,
                    Priority = 5
                },
                
                new FileDescriptionRule
                {
                    Name = "Settings_FlightControl_Communication",
                    PathPattern = "*Settings*Csv*UvhfTadCsv*",
                    Description = "비행통제 통신 관련 설정 파일",
                    Category = FileCategory.Configuration,
                    Priority = 5
                },
                
                new FileDescriptionRule
                {
                    Name = "Settings_FlightControl_General",
                    PathPattern = "*Settings*Csv*",
                    ExcludeConditions = new List<string> { "*CheckCsv*", "*StatusCsv*", "*UvhfTadCsv*" },
                    Description = "비행통제 설정 정보 파일",
                    Category = FileCategory.Configuration,
                    Priority = 15
                },
                
                new FileDescriptionRule
                {
                    Name = "Settings_Mission_Map",
                    PathPattern = "*Settings*Mission*",
                    ExcludeConditions = new List<string> { "*WarSymbol*" },
                    Description = "지도 정보 파일",
                    Category = FileCategory.Data,
                    Priority = 10
                },
                
                new FileDescriptionRule
                {
                    Name = "Settings_Mission_WarSymbol",
                    PathPattern = "*Settings*Mission*WarSymbol*",
                    Description = "지도 표적 이미지 파일",
                    Category = FileCategory.Media,
                    Priority = 5
                },
                
                new FileDescriptionRule
                {
                    Name = "Settings_MouseCursor",
                    PathPattern = "*Settings*MouseCursor*",
                    Description = "TM 마우스 커서 파일",
                    Category = FileCategory.Media,
                    Priority = 10
                },
                
                new FileDescriptionRule
                {
                    Name = "Settings_Media",
                    PathPattern = "*Settings*Wav*",
                    Description = "미디어 파일",
                    Category = FileCategory.Media,
                    Priority = 10
                },
                
                new FileDescriptionRule
                {
                    Name = "Settings_ICD_Excel",
                    PathPattern = "*Settings*Xlsx*",
                    Description = "ICD 정보 관련 파일",
                    Category = FileCategory.Document,
                    Priority = 10
                },
                
                new FileDescriptionRule
                {
                    Name = "Settings_ICD_Xml",
                    PathPattern = "*Settings*Xml*",
                    Description = "ICD 정보 관련 파일",
                    Category = FileCategory.Configuration,
                    Priority = 10
                },

                // === 파일명 기반 규칙 (중간 우선순위) ===
                new FileDescriptionRule
                {
                    Name = "Library_DevExpress",
                    FileNamePattern = "*DevExpress*",
                    Description = "해외구매/UI구성라이브러리",
                    Category = FileCategory.Library,
                    Priority = 20
                },
                
                new FileDescriptionRule
                {
                    Name = "Library_RTIDDS",
                    FileNamePattern = "*ndds*",
                    Description = "해외구매/RTIDDS통신관련라이브러리",
                    Category = FileCategory.Library,
                    Priority = 20
                },
                
                new FileDescriptionRule
                {
                    Name = "Library_NX_Map",
                    FileNamePattern = "*NX*",
                    Description = "국내구매 / 지도관련 엔진 라이브러리",
                    Category = FileCategory.Library,
                    Priority = 20
                },
                
                new FileDescriptionRule
                {
                    Name = "Library_ICD_Common",
                    FileNamePattern = "Module.dll",
                    Description = "연구개발/ICD관련공통모듈",
                    Category = FileCategory.Library,
                    Priority = 15
                },
                
                new FileDescriptionRule
                {
                    Name = "Library_ICD_Model",
                    FileNamePattern = "Model_type.dll",
                    Description = "연구개발/ICD관련공통모듈",
                    Category = FileCategory.Library,
                    Priority = 15
                },
                
                new FileDescriptionRule
                {
                    Name = "Library_ICD_Data",
                    FileNamePattern = "stdatamodel.dll",
                    Description = "연구개발/ICD관련공통모듈",
                    Category = FileCategory.Library,
                    Priority = 15
                },
                
                new FileDescriptionRule
                {
                    Name = "Library_Soletop",
                    FileNamePattern = "Soletop.*",
                    Description = "연구개발 / 공통 라이브러리",
                    Category = FileCategory.Library,
                    Priority = 20
                },

                // === 확장자 기반 규칙 (낮은 우선순위) ===
                new FileDescriptionRule
                {
                    Name = "Executable_General",
                    Extension = ".exe",
                    Description = "연구개발/실행 Exe 파일",
                    Category = FileCategory.Executable,
                    Priority = 50
                },
                
                new FileDescriptionRule
                {
                    Name = "Library_DLL",
                    Extension = ".dll",
                    Description = "연구개발/실행및라이브러리 DLL파일",
                    Category = FileCategory.Library,
                    Priority = 60
                },
                
                // === 기본 규칙 (최하위 우선순위) ===
                new FileDescriptionRule
                {
                    Name = "Default_Environment",
                    PathPattern = "*",
                    Description = "연구개발/ 환경파일",
                    Category = FileCategory.Unknown,
                    Priority = 1000
                }
            };
        }

        /// <summary>
        /// 경로 패턴 매핑을 위한 설정
        /// B.Settings, M.Settings 등을 일괄 처리하기 위한 패턴
        /// </summary>
        public static Dictionary<string, List<string>> GetPathPatternMappings()
        {
            return new Dictionary<string, List<string>>
            {
                ["*Settings*"] = new List<string> { "*B.Settings*", "*M.Settings*", "*A.Settings*", "*C.Settings*" },
                ["*Config*"] = new List<string> { "*Config*", "*Configuration*", "*Cfg*" },
                ["*Mission*"] = new List<string> { "*Mission*", "*Missions*" }
            };
        }

        /// <summary>
        /// 설정 파일에서 규칙 로드 (추후 JSON, XML, DB 연동 가능)
        /// </summary>
        public static List<FileDescriptionRule> LoadFromConfiguration(string configPath = null)
        {
            // TODO: 설정 파일에서 로드하는 로직 구현
            // 현재는 기본 규칙 반환
            return GetDefaultRules();
        }
    }
}