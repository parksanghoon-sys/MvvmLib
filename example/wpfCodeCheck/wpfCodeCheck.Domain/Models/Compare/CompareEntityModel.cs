namespace wpfCodeCheck.Domain.Models.Compare
{
    public record CompareEntityModel
    {
        public string InputFilePath { get; set; } = string.Empty;
        public string OutputFilePath { get; set; } = string.Empty;
        public string InputFileName { get; set; } = string.Empty;
        public string OutputFileName { get; set; } = string.Empty;
    }
}