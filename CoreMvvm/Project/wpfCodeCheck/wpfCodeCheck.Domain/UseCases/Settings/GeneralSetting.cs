using wpfCodeCheck.Domain.Enums;

namespace wpfCodeCheck.Domain.UseCases.Settings
{
    [Serializable]
    public class GeneralSetting
    {
        [Setting("")]
        public string InputPath { get; set; }
        [Setting("")]
        public string OutputPath { get; set; }
        [Setting("")]
        public string OutputExcelPath { get; set; }
        [Setting("")]
        public string OutputExcelFileName { get; set; }
        [Setting("Type")]
        public EType CodeCheckType { get; set; }
    }
}
