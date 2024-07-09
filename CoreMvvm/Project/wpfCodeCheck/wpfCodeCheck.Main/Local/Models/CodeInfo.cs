using CoreMvvmLib.Component.UI.Units;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace wpfCodeCheck.Main.Local.Models
{
    public class CodeInfoCompareer : IEqualityComparer<CodeInfo>
    {
        public bool Equals(CodeInfo? x, CodeInfo? y)
        {
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
        public string? DirectoryPath { get; set; }
        public string? ProjectName { get; set; }
        public string? FilePath { get; set; }
        public string? FileName { get; set; }
        public string? CreateDate { get; set; }
        public string? FileSize { get; set; }
        public int LineCount { get; set; }
        public string? Checksum { get; set; }
        public string? Description { get; set; }
        public string? Csc { get; set; }
        public IconType FileType { get; set; }
        public bool Equals(CodeInfo? other)
        {
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
