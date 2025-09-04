using wpfCodeCheck.Domain.Enums;

namespace wpfCodeCheck.Domain.UseCases.Settings
{
    [Serializable]
    public class GeneralSetting
    {
        [Setting("~ 을 경로")]        
        public string InputPath { get; set; } = string.Empty;
        [Setting("~ 으로 변경 경로")]
        public string OutputPath { get; set; } = string.Empty;
        [Setting("EXCEL 출력 경로")]
        public string OutputExcelPath { get; set; } = string.Empty;
        [Setting("EXCEL 출력 파일 이름")]
        public string OutputExcelFileName { get; set; } = string.Empty;
        [Setting("Type")]
        public ECompareType CompareType { get; set; }
        [Setting("Beyond Output 파일 경로")]
        public string ExportBeyondCompareFilePath { get; set; } = string.Empty;
    }
}
