namespace wpfCodeCheck.Domain.Datas
{
    public class FileItem : FileEntity
    {
        public string CreateDate { get; set; } = string.Empty;
        public string FileSize { get; set; } = string.Empty;
        public int LineCount { get; set; }
        public string Checksum { get; set; } = string.Empty;
        public int Depth { get; set; }        
    }
}
