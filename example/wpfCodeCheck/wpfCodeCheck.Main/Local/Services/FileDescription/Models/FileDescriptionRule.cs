namespace wpfCodeCheck.Main.Local.Services.FileDescription.Models
{
    /// <summary>
    /// 파일 설명 규칙을 정의하는 모델
    /// </summary>
    public class FileDescriptionRule
    {
        /// <summary>
        /// 규칙 이름 (디버깅/로깅용)
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 경로 패턴 (와일드카드 지원)
        /// </summary>
        public string PathPattern { get; set; } = string.Empty;

        /// <summary>
        /// 파일명 패턴 (와일드카드 지원)
        /// </summary>
        public string FileNamePattern { get; set; } = string.Empty;

        /// <summary>
        /// 파일 확장자 (우선순위 낮음)
        /// </summary>
        public string Extension { get; set; } = string.Empty;

        /// <summary>
        /// 매칭 시 반환할 설명
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 규칙 우선순위 (낮을수록 높은 우선순위)
        /// </summary>
        public int Priority { get; set; } = 100;

        /// <summary>
        /// 규칙 활성화 여부
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// 추가 매칭 조건들 (AND 조건)
        /// </summary>
        public List<string> AdditionalConditions { get; set; } = new();

        /// <summary>
        /// 제외 조건들 (이 조건들 중 하나라도 매칭되면 제외)
        /// </summary>
        public List<string> ExcludeConditions { get; set; } = new();
        public FileCategory Category { get; internal set; }

        public override string ToString()
        {
            return $"Rule: {Name} - {Description} (Priority: {Priority})";
        }
    }

    /// <summary>
    /// 파일 분류 카테고리
    /// </summary>
    public enum FileCategory
    {
        Executable,         // 실행 파일
        Library,           // 라이브러리
        Configuration,     // 설정 파일
        Data,             // 데이터 파일
        Media,            // 미디어 파일
        Document,         // 문서 파일
        Unknown           // 알 수 없음
    }

    /// <summary>
    /// 파일 설명 결과
    /// </summary>
    public class FileDescriptionResult
    {
        public string Description { get; set; } = "알 수 없는 파일";
        public FileCategory Category { get; set; } = FileCategory.Unknown;
        public string MatchedRuleName { get; set; } = "None";
        public int MatchedRulePriority { get; set; } = int.MaxValue;
    }
}