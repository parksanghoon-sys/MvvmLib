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

        // 비교 대상 파일 정보 (비교 결과에서 사용)
        public string CompareFilePath { get; set; } = string.Empty;
        public string CompareFileName { get; set; } = string.Empty;
        
        // 비교 상태
        public bool IsComparison
        {
            get => _isComparison;
            set => SetProperty(ref _isComparison, value);
        }

        // 파일 존재 상태 (INPUT만, OUTPUT만, 둘다 있음)
        public CompareStatus Status { get; set; } = CompareStatus.Both;
    }

    public enum CompareStatus
    {
        InputOnly,   // INPUT에만 존재
        OutputOnly,  // OUTPUT에만 존재
        Both,        // 양쪽에 존재 (비교 가능)
        Different    // 양쪽에 존재하지만 다름
    }
}