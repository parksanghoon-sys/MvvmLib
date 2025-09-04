using System.Text.RegularExpressions;

namespace wpfCodeCheck.Main.Local.Services.FileDescription.Matching
{
    /// <summary>
    /// 경로 및 파일명 패턴 매칭 유틸리티
    /// </summary>
    public static class PatternMatcher
    {
        /// <summary>
        /// 와일드카드 패턴을 정규식으로 변환하여 매칭
        /// </summary>
        public static bool IsWildcardMatch(string input, string pattern)
        {
            if (string.IsNullOrEmpty(pattern) || string.IsNullOrEmpty(input))
                return false;

            // 와일드카드 패턴을 정규식으로 변환
            var regexPattern = WildcardToRegex(pattern);
            
            return Regex.IsMatch(input, regexPattern, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 여러 패턴 중 하나라도 매칭되는지 확인
        /// </summary>
        public static bool IsAnyPatternMatch(string input, IEnumerable<string> patterns)
        {
            return patterns.Any(pattern => IsWildcardMatch(input, pattern));
        }

        /// <summary>
        /// 모든 패턴이 매칭되는지 확인 (AND 조건)
        /// </summary>
        public static bool AreAllPatternsMatch(string input, IEnumerable<string> patterns)
        {
            return patterns.All(pattern => IsWildcardMatch(input, pattern));
        }

        /// <summary>
        /// 경로 구분자를 통일하여 매칭 (Windows/Unix 호환)
        /// </summary>
        public static bool IsPathPatternMatch(string filePath, string pattern)
        {
            if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(pattern))
                return false;

            // 경로 구분자 통일 (백슬래시를 슬래시로)
            var normalizedPath = NormalizePath(filePath);
            var normalizedPattern = NormalizePath(pattern);

            return IsWildcardMatch(normalizedPath, normalizedPattern);
        }

        /// <summary>
        /// 와일드카드 패턴을 정규식으로 변환
        /// </summary>
        private static string WildcardToRegex(string pattern)
        {
            // 정규식 특수문자 이스케이프
            var escaped = Regex.Escape(pattern)
                .Replace("\\*", ".*")  // * -> .*
                .Replace("\\?", ".");  // ? -> .

            return $"^{escaped}$";
        }

        /// <summary>
        /// 경로 정규화 (구분자 통일)
        /// </summary>
        private static string NormalizePath(string path)
        {
            return path.Replace('\\', '/').Replace("//", "/");
        }

        /// <summary>
        /// 패턴 매핑을 적용하여 확장된 패턴 생성
        /// 예: "*Settings*" -> ["*B.Settings*", "*M.Settings*", ...]
        /// </summary>
        public static List<string> ExpandPatternWithMappings(string originalPattern, 
            Dictionary<string, List<string>> patternMappings)
        {
            var expandedPatterns = new List<string> { originalPattern };

            foreach (var mapping in patternMappings)
            {
                if (originalPattern.Contains(mapping.Key))
                {
                    var newPatterns = new List<string>();
                    
                    foreach (var existingPattern in expandedPatterns)
                    {
                        foreach (var replacement in mapping.Value)
                        {
                            var newPattern = existingPattern.Replace(mapping.Key, replacement);
                            newPatterns.Add(newPattern);
                        }
                    }
                    
                    expandedPatterns = newPatterns;
                }
            }

            return expandedPatterns.Distinct().ToList();
        }
    }

    /// <summary>
    /// 파일 정보 매칭 결과
    /// </summary>
    public class MatchResult
    {
        public bool IsMatch { get; set; }
        public string MatchedPattern { get; set; } = string.Empty;
        public MatchType MatchType { get; set; }
        public int MatchStrength { get; set; } // 매칭 강도 (더 구체적인 매칭일수록 높음)

        public static MatchResult NoMatch => new MatchResult { IsMatch = false };
        
        public static MatchResult Match(string pattern, MatchType type, int strength = 1) => 
            new MatchResult 
            { 
                IsMatch = true, 
                MatchedPattern = pattern, 
                MatchType = type,
                MatchStrength = strength
            };
    }

    /// <summary>
    /// 매칭 타입
    /// </summary>
    public enum MatchType
    {
        PathPattern,      // 경로 패턴 매칭
        FileNamePattern,  // 파일명 패턴 매칭
        Extension,        // 확장자 매칭
        Exact            // 정확한 매칭
    }
}