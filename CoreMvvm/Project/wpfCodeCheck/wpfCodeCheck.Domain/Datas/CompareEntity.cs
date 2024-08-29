namespace wpfCodeCheck.Domain.Datas
{
    public record CompareEntity
    {
        public string InputFilePath { get; set; } = string.Empty;
        public string OutoutFilePath { get; set; } = string.Empty;
        public string InputFileName { get; set; } = string.Empty;
        public string OutoutFileName { get; set; } = string.Empty;
    }
}
