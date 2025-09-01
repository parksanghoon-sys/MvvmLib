using wpfCodeCheck.Domain.Models;

namespace wpfCodeCheck.Main.Local.Models
{
    /// <summary>
    /// 비교 결과를 전달하는 메시지
    /// </summary>
    public class ComparisonResultMessage
    {
        /// <summary>
        /// 비교 완료된 파일들 (IsComparison 값이 업데이트됨)
        /// </summary>
        public List<FileTreeModel> ComparedFiles { get; set; } = new();

        /// <summary>
        /// 차이점이 있는 파일들만
        /// </summary>
        public List<FileTreeModel> DifferenceFiles { get; set; } = new();

        /// <summary>
        /// 비교 타입 (Input/Output 구분용)
        /// </summary>
        public string ComparisonType { get; set; } = string.Empty;

        public ComparisonResultMessage(List<FileTreeModel> comparedFiles, List<FileTreeModel> differenceFiles, string comparisonType)
        {
            ComparedFiles = comparedFiles;
            DifferenceFiles = differenceFiles;
            ComparisonType = comparisonType;
        }
    }
}