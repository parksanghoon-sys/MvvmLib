using System.Diagnostics;
using System.IO;
using wpfCodeCheck.Main.Local.Services.FileDescription.Configuration;
using wpfCodeCheck.Main.Local.Services.FileDescription.Matching;
using wpfCodeCheck.Main.Local.Services.FileDescription.Models;

namespace wpfCodeCheck.Main.Local.Services.FileDescription
{
    /// <summary>
    /// 파일 설명 생성 서비스 구현
    /// OOP 원칙을 적용한 확장 가능한 구조
    /// </summary>
    public class FileDescriptionService : IFileDescriptionService
    {
        private readonly List<FileDescriptionRule> _rules;
        private readonly Dictionary<string, List<string>> _pathPatternMappings;
        private readonly FileDescriptionStatistics _statistics;
        private readonly object _lockObject = new();

        public FileDescriptionService()
        {
            _rules = new List<FileDescriptionRule>();
            _pathPatternMappings = FileDescriptionConfig.GetPathPatternMappings();
            _statistics = new FileDescriptionStatistics();
            
            // 기본 규칙 로드
            ReloadRules();
        }

        public FileDescriptionResult GetDescription(string filePath, string fileName)
        {
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                var result = ProcessSingleFile(filePath, fileName);
                
                // 통계 업데이트
                UpdateStatistics(result, stopwatch.Elapsed);
                
                return result;
            }
            catch (Exception ex)
            {
                // 로그 남기기 (실제로는 ILogger 사용 권장)
                Console.WriteLine($"Error getting description for {fileName}: {ex.Message}");
                
                return new FileDescriptionResult
                {
                    Description = "오류 발생",
                    Category = FileCategory.Unknown,
                    MatchedRuleName = "Error"
                };
            }
            finally
            {
                stopwatch.Stop();
            }
        }

        public Dictionary<string, FileDescriptionResult> GetDescriptions(IEnumerable<(string filePath, string fileName)> files)
        {
            var results = new Dictionary<string, FileDescriptionResult>();
            
            foreach (var (filePath, fileName) in files)
            {
                var key = $"{filePath}|{fileName}";
                results[key] = GetDescription(filePath, fileName);
            }
            
            return results;
        }

        public void AddRule(FileDescriptionRule rule)
        {
            lock (_lockObject)
            {
                // 중복 규칙 체크
                if (_rules.Any(r => r.Name.Equals(rule.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new ArgumentException($"Rule with name '{rule.Name}' already exists.");
                }
                
                _rules.Add(rule);
                
                // 우선순위로 정렬
                _rules.Sort((a, b) => a.Priority.CompareTo(b.Priority));
                
                _statistics.TotalRules = _rules.Count;
                _statistics.EnabledRules = _rules.Count(r => r.IsEnabled);
            }
        }

        public bool RemoveRule(string ruleName)
        {
            lock (_lockObject)
            {
                var rule = _rules.FirstOrDefault(r => r.Name.Equals(ruleName, StringComparison.OrdinalIgnoreCase));
                if (rule != null)
                {
                    _rules.Remove(rule);
                    
                    _statistics.TotalRules = _rules.Count;
                    _statistics.EnabledRules = _rules.Count(r => r.IsEnabled);
                    
                    return true;
                }
                return false;
            }
        }

        public IReadOnlyList<FileDescriptionRule> GetAllRules()
        {
            lock (_lockObject)
            {
                return _rules.ToList().AsReadOnly();
            }
        }

        public void ReloadRules()
        {
            lock (_lockObject)
            {
                _rules.Clear();
                
                var defaultRules = FileDescriptionConfig.LoadFromConfiguration();
                _rules.AddRange(defaultRules);
                
                // 우선순위로 정렬
                _rules.Sort((a, b) => a.Priority.CompareTo(b.Priority));
                
                _statistics.TotalRules = _rules.Count;
                _statistics.EnabledRules = _rules.Count(r => r.IsEnabled);
            }
        }

        public FileDescriptionStatistics GetStatistics()
        {
            lock (_lockObject)
            {
                return new FileDescriptionStatistics
                {
                    TotalRules = _statistics.TotalRules,
                    EnabledRules = _statistics.EnabledRules,
                    TotalMatches = _statistics.TotalMatches,
                    MatchesByCategory = new Dictionary<string, int>(_statistics.MatchesByCategory),
                    MatchesByRule = new Dictionary<string, int>(_statistics.MatchesByRule),
                    AverageMatchTime = _statistics.AverageMatchTime
                };
            }
        }

        /// <summary>
        /// 단일 파일에 대한 설명 처리
        /// </summary>
        private FileDescriptionResult ProcessSingleFile(string filePath, string fileName)
        {
            foreach (var rule in _rules.Where(r => r.IsEnabled))
            {
                if (IsRuleMatch(rule, filePath, fileName))
                {
                    return new FileDescriptionResult
                    {
                        Description = rule.Description,
                        Category = rule.Category,
                        MatchedRuleName = rule.Name,
                        MatchedRulePriority = rule.Priority
                    };
                }
            }

            // 기본값 반환
            return new FileDescriptionResult();
        }

        /// <summary>
        /// 규칙이 파일과 매칭되는지 확인
        /// </summary>
        private bool IsRuleMatch(FileDescriptionRule rule, string filePath, string fileName)
        {
            // 제외 조건 확인 (먼저 체크)
            if (rule.ExcludeConditions.Any())
            {
                foreach (var excludeCondition in rule.ExcludeConditions)
                {
                    if (PatternMatcher.IsPathPatternMatch(filePath, excludeCondition) ||
                        PatternMatcher.IsWildcardMatch(fileName, excludeCondition))
                    {
                        return false; // 제외 조건에 매칭되면 규칙 적용 안함
                    }
                }
            }

            // 경로 패턴 확인
            if (!string.IsNullOrEmpty(rule.PathPattern))
            {
                var expandedPatterns = PatternMatcher.ExpandPatternWithMappings(rule.PathPattern, _pathPatternMappings);
                
                if (!expandedPatterns.Any(pattern => PatternMatcher.IsPathPatternMatch(filePath, pattern)))
                {
                    return false;
                }
            }

            // 파일명 패턴 확인
            if (!string.IsNullOrEmpty(rule.FileNamePattern))
            {
                if (!PatternMatcher.IsWildcardMatch(fileName, rule.FileNamePattern))
                {
                    return false;
                }
            }

            // 확장자 확인
            if (!string.IsNullOrEmpty(rule.Extension))
            {
                var fileExtension = Path.GetExtension(fileName);
                if (!string.Equals(fileExtension, rule.Extension, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            // 추가 조건 확인 (AND 조건)
            if (rule.AdditionalConditions.Any())
            {
                foreach (var condition in rule.AdditionalConditions)
                {
                    if (!PatternMatcher.IsPathPatternMatch(filePath, condition) &&
                        !PatternMatcher.IsWildcardMatch(fileName, condition))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 통계 정보 업데이트
        /// </summary>
        private void UpdateStatistics(FileDescriptionResult result, TimeSpan elapsed)
        {
            lock (_lockObject)
            {
                _statistics.TotalMatches++;

                // 카테고리별 통계
                var categoryKey = result.Category.ToString();
                _statistics.MatchesByCategory.TryGetValue(categoryKey, out var categoryCount);
                _statistics.MatchesByCategory[categoryKey] = categoryCount + 1;

                // 규칙별 통계
                _statistics.MatchesByRule.TryGetValue(result.MatchedRuleName, out var ruleCount);
                _statistics.MatchesByRule[result.MatchedRuleName] = ruleCount + 1;

                // 평균 처리 시간 계산
                var totalTicks = _statistics.AverageMatchTime.Ticks * (_statistics.TotalMatches - 1) + elapsed.Ticks;
                _statistics.AverageMatchTime = new TimeSpan(totalTicks / _statistics.TotalMatches);
            }
        }
    }
}