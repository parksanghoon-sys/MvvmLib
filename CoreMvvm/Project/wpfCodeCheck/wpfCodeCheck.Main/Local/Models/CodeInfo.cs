using CoreMvvmLib.Component.UI.Units;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace wpfCodeCheck.Main.Local.Models
{
    public class CodeInfoCompareer : IEqualityComparer<CodeInfo>
    {
        public bool Equals(CodeInfo? x, CodeInfo? y)
        {
            if (ReferenceEquals(x, null) || ReferenceEquals(y,null))
                return false;            
            return (x.Checksum == y.Checksum) && (x.FileName == y.FileName);
        }

        public int GetHashCode([DisallowNull] CodeInfo obj)
        {
            return (obj.Checksum, obj.FileName).GetHashCode();
        }
    }
    public class CodeInfo : IEquatable<CodeInfo>, INotifyPropertyChanged
    {
        private bool _comparisonResult;

        public bool ComparisonResult
        {
            get { return _comparisonResult; }
            set { _comparisonResult = value; OnPropertyChanged(); }
        }
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
        public IconType FileType { get; set; }
        public bool Equals(CodeInfo? other)
        {
            if (ReferenceEquals(null, other))
                return false;
            return (this.Checksum == other.Checksum && this.FileName == other.FileName);
        }
        public override string ToString()
        {
            return $"{FileName} | {Checksum} | {LineCount}";
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
