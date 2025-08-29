using wpfCodeCheck.Domain.Enums;

namespace wpfCodeCheck.Domain.UseCases.Settings
{
    [Serializable]
    public class GeneralSetting
    {
        [Setting("~ 을 경로")]        
        public string InputPath { get; set; }
        [Setting("~ 으로 변경 경로")]
        public string OutputPath { get; set; }
        [Setting("EXCEL 출력 경로")]
        public string OutputExcelPath { get; set; }
        [Setting("EXCEL 출력 파일 이름")]
        public string OutputExcelFileName { get; set; }
        [Setting("Type")]
        public ECompareType CompareType { get; set; }
    }
}
