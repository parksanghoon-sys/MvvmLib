using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace wpfCodeCheck.Domain.Datas
{
    public class FileCompareModel : FileItem, IEquatable<FileItem>
    {        
        public bool IsComparison { get; set; }   

        public int FileIndex { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public int Depth { get; set; }
        public virtual List<FileCompareModel>? Children { get; set; }

        public bool Equals(FileItem? other)
        {
            if (ReferenceEquals(null, other))
                return false;
            return (this.Checksum == other.Checksum && this.FileName == other.FileName && this.CreateDate == other.CreateDate);
        }
        public override string ToString()
        {
            return $"{IsComparison}|{FileName} | {Checksum} | {LineCount}";
        }     
    }
}
