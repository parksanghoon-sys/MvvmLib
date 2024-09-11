namespace wpfCodeCheck.Domain.Datas
{
    public class FileItem : FileEntity
    {
        public string CreateDate { get; set; } = string.Empty;
        public long? FileSize { get; set; }
        public int LineCount { get; set; }
        public string Checksum { get; set; } = string.Empty;        
        public string Description { get; set; } = string.Empty;
    }
}
