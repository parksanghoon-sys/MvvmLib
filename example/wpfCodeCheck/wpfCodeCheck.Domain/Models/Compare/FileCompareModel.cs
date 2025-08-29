using wpfCodeCheck.Domain.Models.Base;
using wpfCodeCheck.Domain.Enums;

namespace wpfCodeCheck.Domain.Models.Compare
{
    public class FileCompareModel : BaseModel
    {
        private bool _isComparison;

        public int FileIndex { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string CreateDate { get; set; } = string.Empty;
        public long? FileSize { get; set; }
        public int LineCount { get; set; }
        public string Checksum { get; set; } = string.Empty;
        public EFileType FileType { get; set; }

        public bool IsComparison
        {
            get => _isComparison;
            set => SetProperty(ref _isComparison, value);
        }
    }
}