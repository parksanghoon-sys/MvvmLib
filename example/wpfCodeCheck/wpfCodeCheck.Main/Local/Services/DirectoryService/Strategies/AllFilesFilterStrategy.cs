using System.IO;

namespace wpfCodeCheck.Main.Local.Services.DirectoryService.Strategies
{
    /// <summary>
    /// 모든 파일 필터링 전략 (기본 시스템 폴더만 제외)
    /// </summary>
    public class AllFilesFilterStrategy : IFileFilterStrategy
    {
        public string StrategyName => "AllFiles";

        public string[] FilePatterns => new[] { "*.*" };

        public string[] ExcludedFileNames => Array.Empty<string>();

        public string[] ExcludedFolders => new[] 
        { 
            "Debug", "Release", "bin", "obj", ".svn", ".git", ".vs" 
        };

        public bool IsFileAllowed(FileInfo fileInfo)
        {
            // 모든 파일 허용 (제외 파일명이 없으므로)
            return true;
        }

        public bool IsFolderAllowed(DirectoryInfo directoryInfo)
        {
            // 기본 시스템/빌드 폴더만 제외
            if (ExcludedFolders.Contains(directoryInfo.Name, StringComparer.OrdinalIgnoreCase))
                return false;

            // 폴더 경로에 제외 폴더가 포함되어 있는지 확인
            return !ExcludedFolders.Any(excludedFolder =>
                directoryInfo.FullName.Contains($"{Path.DirectorySeparatorChar}{excludedFolder}{Path.DirectorySeparatorChar}", 
                    StringComparison.OrdinalIgnoreCase));
        }
    }
}