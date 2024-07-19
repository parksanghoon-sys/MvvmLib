namespace wpfCodeCheck.Domain.UseCases.Settings
{
    [Serializable]
    public class GeneralSetting
    {
        [Setting("")]
        public string InputPath { get; set; }
        [Setting("")]
        public string OutputPath { get; set; }
    }
}
