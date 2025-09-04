using wpfCodeCheck.Main.Local.Services.FileDescription.Models;

namespace wpfCodeCheck.Main.Local.Services.FileDescription
{
    /// <summary>
    /// FileDescriptionService 사용 예제 (개발/디버깅용)
    /// </summary>
    internal static class QuickDemo
    {
        /// <summary>
        /// 콘솔에서 실행 가능한 간단한 데모
        /// </summary>
        public static void RunQuickDemo()
        {
            Console.WriteLine("=== FileDescriptionService 빠른 데모 ===\n");
            
            // 서비스 생성 (DI 없이 직접 생성)
            var service = new FileDescriptionService();
            
            // 테스트 케이스들 - 기존 하드코딩 문제를 해결하는 예시
            var testCases = new[]
            {
                (@"C:\App\B.Settings\Config\main.config", "main.config"),
                (@"C:\App\M.Settings\Config\database.config", "database.config"),
                (@"C:\App\A.Settings\Config\network.config", "network.config"), // 새로운 패턴도 자동 지원
                (@"C:\App\B.Settings\Csv\AutoSatProfile\satellite.csv", "satellite.csv"),
                (@"C:\App\B.Settings\Csv\StatusCsv\status.csv", "status.csv"),
                (@"C:\App\B.Settings\Csv\UvhfTadCsv\communication.csv", "communication.csv"),
                (@"C:\App\B.Settings\Mission\map.xml", "map.xml"),
                (@"C:\App\M.Settings\Mission\WarSymbol\target.png", "target.png"),
                (@"C:\Program Files\Lib\DevExpress.dll", "DevExpress.dll"),
                (@"C:\Program Files\Lib\ndds.dll", "ndds.dll"),
                (@"C:\Program Files\App\main.exe", "main.exe"),
                (@"C:\Program Files\Data\unknown.txt", "unknown.txt")
            };

            Console.WriteLine("| 파일명 | 경로 패턴 | 설명 | 카테고리 | 매칭 규칙 |");
            Console.WriteLine("|--------|-----------|------|----------|-----------|");

            foreach (var (path, fileName) in testCases)
            {
                var result = service.GetDescription(path, fileName);
                var pathPattern = ExtractPattern(path);
                
                Console.WriteLine($"| {fileName} | {pathPattern} | {result.Description} | {result.Category} | {result.MatchedRuleName} |");
            }

            // 성능 및 통계 정보
            var stats = service.GetStatistics();
            Console.WriteLine($"\n=== 통계 정보 ===");
            Console.WriteLine($"총 규칙 수: {stats.TotalRules}");
            Console.WriteLine($"활성 규칙 수: {stats.EnabledRules}");
            Console.WriteLine($"총 매칭 수: {stats.TotalMatches}");
            Console.WriteLine($"평균 매칭 시간: {stats.AverageMatchTime.TotalMilliseconds:F2}ms");
            
            Console.WriteLine($"\n=== 개선점 요약 ===");
            Console.WriteLine("✅ B.Settings, M.Settings, A.Settings, C.Settings 자동 매핑");
            Console.WriteLine("✅ 우선순위 기반 정확한 매칭 (복잡한 if-else 제거)");
            Console.WriteLine("✅ 런타임 규칙 추가/제거 가능");
            Console.WriteLine("✅ 설정 파일 기반 관리로 유지보수성 향상");
            Console.WriteLine("✅ 통계 정보로 성능 모니터링 가능");
            Console.WriteLine("✅ 단위 테스트 가능한 구조");
        }

        /// <summary>
        /// 새 규칙 추가 데모
        /// </summary>
        public static void DemoCustomRules()
        {
            Console.WriteLine("\n=== 커스텀 규칙 추가 데모 ===");
            
            var service = new FileDescriptionService();
            
            // 새로운 규칙 추가 - 로그 파일 처리
            var logRule = new FileDescriptionRule
            {
                Name = "Custom_Log_Files",
                PathPattern = "*Logs*",
                Extension = ".log",
                Description = "시스템 로그 파일",                
                Priority = 5
            };

            service.AddRule(logRule);
            
            // 테스트
            var result = service.GetDescription(@"C:\App\Logs\system.log", "system.log");
            Console.WriteLine($"커스텀 규칙 테스트: {result.Description} (규칙: {result.MatchedRuleName})");
        }

        /// <summary>
        /// 패턴 매핑 시스템 데모
        /// </summary>
        public static void DemoPatternMapping()
        {
            Console.WriteLine("\n=== 패턴 매핑 시스템 데모 ===");
            
            var service = new FileDescriptionService();
            
            // 같은 파일이 다른 Settings 폴더에 있을 때
            var settingsVariations = new[]
            {
                @"C:\App\B.Settings\Config\main.config",
                @"C:\App\M.Settings\Config\main.config",
                @"C:\App\A.Settings\Config\main.config",
                @"C:\App\C.Settings\Config\main.config"
            };

            Console.WriteLine("동일한 파일의 다양한 Settings 폴더 매핑 테스트:");
            foreach (var path in settingsVariations)
            {
                var result = service.GetDescription(path, "main.config");
                Console.WriteLine($"  {ExtractPattern(path)} → {result.Description}");
            }
        }

        private static string ExtractPattern(string path)
        {
            if (path.Contains("B.Settings")) return "*B.Settings*";
            if (path.Contains("M.Settings")) return "*M.Settings*";
            if (path.Contains("A.Settings")) return "*A.Settings*";
            if (path.Contains("C.Settings")) return "*C.Settings*";
            return "*";
        }

        /// <summary>
        /// 전체 데모 실행 (개발자용)
        /// </summary>
        public static void RunFullDemo()
        {
            try
            {
                RunQuickDemo();
                DemoCustomRules();
                DemoPatternMapping();
                Console.WriteLine("\n✅ FileDescriptionService 데모 완료!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 데모 실행 오류: {ex.Message}");
            }
        }
    }
}