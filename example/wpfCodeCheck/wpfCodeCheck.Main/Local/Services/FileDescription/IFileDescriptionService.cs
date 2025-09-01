using wpfCodeCheck.Main.Local.Services.FileDescription.Models;

namespace wpfCodeCheck.Main.Local.Services.FileDescription
{
    /// <summary>
    /// 파일 설명 생성 서비스 인터페이스
    /// </summary>
    public interface IFileDescriptionService
    {
        /// <summary>
        /// 파일 경로와 이름을 기반으로 설명 생성
        /// </summary>
        FileDescriptionResult GetDescription(string filePath, string fileName);

        /// <summary>
        /// 여러 파일에 대한 설명 배치 생성
        /// </summary>
        Dictionary<string, FileDescriptionResult> GetDescriptions(IEnumerable<(string filePath, string fileName)> files);

        /// <summary>
        /// 규칙 추가
        /// </summary>
        void AddRule(FileDescriptionRule rule);

        /// <summary>
        /// 규칙 제거
        /// </summary>
        bool RemoveRule(string ruleName);

        /// <summary>
        /// 모든 규칙 조회
        /// </summary>
        IReadOnlyList<FileDescriptionRule> GetAllRules();

        /// <summary>
        /// 규칙 다시 로드
        /// </summary>
        void ReloadRules();

        /// <summary>
        /// 통계 정보 조회
        /// </summary>
        FileDescriptionStatistics GetStatistics();
    }

    /// <summary>
    /// 파일 설명 서비스 통계
    /// </summary>
    public class FileDescriptionStatistics
    {
        public int TotalRules { get; set; }
        public int EnabledRules { get; set; }
        public int TotalMatches { get; set; }
        public Dictionary<string, int> MatchesByCategory { get; set; } = new();
        public Dictionary<string, int> MatchesByRule { get; set; } = new();
        public TimeSpan AverageMatchTime { get; set; }
    }
}