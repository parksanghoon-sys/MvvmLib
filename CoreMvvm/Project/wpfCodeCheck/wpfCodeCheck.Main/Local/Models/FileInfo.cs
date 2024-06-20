namespace wpfCodeCheck.Main.Local.Models
{
    public enum FileDef { Source, UI, Image, Config, Header, EXE, dll, Setting }
    public class CodeInfo : IEquatable<CodeInfo>
    {
        public string? ProjectName { get; set; }
        public string? FilePath { get; set; }
        public string? FileName { get; set; }        
        public string? CreateDate { get; set; }
        public string? FileSize { get; set; }
        public int LineCount { get; set; }
        public string? Checksum { get; set; }
        public string? Description { get; set; }
        public string? Csc { get; set; }
        public FileDef FileType { get; set; }
        public bool ComparisonResult { get; set; } = false;
        public bool Equals(CodeInfo? other)
        {
            return !(this.Checksum == other.Checksum && this.FileName == other.FileName);
        }
        public override string ToString()
        {
            return $"{FileName} | {Checksum}";
        }
    }
}
