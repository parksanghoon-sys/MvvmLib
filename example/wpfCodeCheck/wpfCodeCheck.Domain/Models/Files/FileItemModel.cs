namespace wpfCodeCheck.Domain.Models.Files
{
    public class FileItemModel : FileEntityModel
    {
        public string CreateDate { get; set; } = string.Empty;
        public long? FileSize { get; set; }
        public int LineCount { get; set; }
        public string Checksum { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}