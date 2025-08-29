using wpfCodeCheck.Domain.Models.Base;
using wpfCodeCheck.Domain.Enums;

namespace wpfCodeCheck.Domain.Models
{
    /// <summary>
    /// 파일/디렉토리 정보와 비교 상태를 포함한 통합 모델
    /// </summary>
    public class FileTreeModel : BaseModel
    {
        private bool _isComparison = true;
        private bool _isExpanded = false;
        private bool _isSelected = false;

        #region 기본 파일 정보
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string CreateDate { get; set; } = string.Empty;
        public long? FileSize { get; set; }
        public int LineCount { get; set; }
        public string Checksum { get; set; } = string.Empty;
        public int Depth { get; set; }
        #endregion

        #region 트리 구조
        public List<FileTreeModel> Children { get; set; } = new List<FileTreeModel>();
        public FileTreeModel? Parent { get; set; }
        public bool IsDirectory => Children.Any() || (!string.IsNullOrEmpty(FilePath) && Directory.Exists(FilePath));
        public bool HasChildren => Children.Count > 0;
        #endregion

        #region 비교 상태
        /// <summary>
        /// 파일이 동일한지 여부 (false이면 차이가 있음)
        /// </summary>
        public bool IsComparison
        {
            get => _isComparison;
            set => SetProperty(ref _isComparison, value);
        }

        /// <summary>
        /// 비교 대상 파일 (Input/Output 비교 시)
        /// </summary>
        public FileTreeModel? CompareTarget { get; set; }

        /// <summary>
        /// 파일 존재 상태
        /// </summary>
        public CompareStatus Status { get; set; } = CompareStatus.Both;
        #endregion

        #region UI 속성
        /// <summary>
        /// 트리뷰 확장 상태
        /// </summary>
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        /// <summary>
        /// 선택 상태
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        /// <summary>
        /// 파일 타입에 따른 아이콘
        /// </summary>
        public IconType FileType => GetFileType(FileName);
        #endregion

        #region 헬퍼 메서드
        /// <summary>
        /// 파일 확장자에 따른 아이콘 타입 반환
        /// </summary>
        private IconType GetFileType(string fileName)
        {
            if (IsDirectory)
                return IconType.File; // 폴더 아이콘

            var extension = Path.GetExtension(fileName).ToLower();
            return extension switch
            {
                ".cs" or ".cpp" or ".cxx" => IconType.Code,
                ".xaml" => IconType.ConsoleLine,
                ".png" or ".jpg" or ".jpeg" => IconType.Image,
                ".exe" => IconType.File,
                ".dll" => IconType.File,
                ".h" => IconType.File,
                _ => IconType.Comment
            };
        }

        /// <summary>
        /// 모든 자식 노드를 평면화하여 반환
        /// </summary>
        public IEnumerable<FileTreeModel> GetAllDescendants()
        {
            foreach (var child in Children)
            {
                yield return child;
                foreach (var descendant in child.GetAllDescendants())
                {
                    yield return descendant;
                }
            }
        }

        /// <summary>
        /// 차이가 있는 파일들만 반환
        /// </summary>
        public IEnumerable<FileTreeModel> GetDifferentFiles()
        {
            if (!IsComparison && !IsDirectory)
                yield return this;

            foreach (var different in Children.SelectMany(child => child.GetDifferentFiles()))
            {
                yield return different;
            }
        }

        /// <summary>
        /// 루트까지의 경로를 문자열로 반환
        /// </summary>
        public string GetFullPath()
        {
            if (Parent == null)
                return FileName;
            return Path.Combine(Parent.GetFullPath(), FileName);
        }
        #endregion

        public override string ToString()
        {
            return $"{FileName} ({Status}) - Compare: {IsComparison}";
        }
    }

    public enum CompareStatus
    {
        /// <summary>입력 디렉토리에만 존재</summary>
        InputOnly,
        /// <summary>출력 디렉토리에만 존재</summary>
        OutputOnly,
        /// <summary>양쪽에 존재 (비교 가능)</summary>
        Both,
        /// <summary>양쪽에 존재하지만 내용이 다름</summary>
        Different
    }
}