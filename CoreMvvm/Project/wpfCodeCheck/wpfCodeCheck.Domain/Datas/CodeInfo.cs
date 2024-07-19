namespace wpfCodeCheck.Domain.Datas
{
    public class CodeInfo
    {        
        public string DirectoryPath { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string CreateDate { get; set; } = string.Empty;
        public string FileSize { get; set; } = string.Empty;
        public int LineCount { get; set; }
        public string Checksum { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Csc { get; set; } = string.Empty;
               
    }
}
