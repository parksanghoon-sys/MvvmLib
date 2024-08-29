using CoreMvvmLib.Component.UI.Units;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using wpfCodeCheck.Domain.Datas;

namespace wpfCodeCheck.Main.Local.Models
{
    public class CodeInfoModel : CodeInfo  ,IEquatable<CodeInfo>, INotifyPropertyChanged
    {
        private bool _comparisonResult;        
        public bool ComparisonResult
        {
            get { return _comparisonResult; }
            set { _comparisonResult = value; OnPropertyChanged(); }
        }
        public IconType FileType { get; set; }
        public bool Equals(CodeInfo? other)
        {
            if (ReferenceEquals(null, other))
                return false;
            return (this.Checksum == other.Checksum && this.FileName == other.FileName && this.CreateDate == other.CreateDate);
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
