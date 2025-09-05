using System.IO;

namespace wpfCodeCheck.Main.Local.Services.DirectoryService.Strategies
{
    /// <summary>
    /// 소스코드 파일 필터링 전략
    /// </summary>
    public class SourceCodeFilterStrategy : IFileFilterStrategy
    {
        public string StrategyName => "SourceCode";

        public string[] FilePatterns => new[] 
        { 
            "*.cxx", "*.cpp", "*.h", "*.cs", "*.xaml", "*.c"
        };

        public string[] ExcludedFileNames => new[] 
        { 
            "App.xaml.cs", "App.xaml", "AssemblyInfo.cs", 
            "Resources.Designer.cs", "Settings.Designer.cs", 
            "AssemblyAttributes.cs", "ms-persist.xml", "packages.config" 
        };

        public string[] ExcludedFolders => new[] 
        { 
            "Debug", "Release", "bin", "obj", ".svn", ".git", ".vs", 
            "Properties", "LogHelper.Net.Framework" ,"LibTest", "intermediate"
        };

        public bool IsFileAllowed(FileInfo fileInfo)
        {
            // 제외 파일명 확인
            if (ExcludedFileNames.Contains(fileInfo.Name, StringComparer.OrdinalIgnoreCase))
                return false;

            // 파일 확장자 (.cs, .cpp 등)
            var extension = fileInfo.Extension;

            // 패턴에서 확장자만 추출해서 비교
            return FilePatterns.Any(pattern =>
                string.Equals(Path.GetExtension(pattern), extension, StringComparison.OrdinalIgnoreCase));
        }


        public bool IsFolderAllowed(DirectoryInfo directoryInfo)
        {
            // 제외 폴더 확인
            if (ExcludedFolders.Contains(directoryInfo.Name, StringComparer.OrdinalIgnoreCase))
                return false;

            // 폴더 경로에 제외 폴더가 포함되어 있는지 확인
            return !ExcludedFolders.Any(excludedFolder =>
                directoryInfo.FullName.Contains($"{Path.DirectorySeparatorChar}{excludedFolder}{Path.DirectorySeparatorChar}", 
                    StringComparison.OrdinalIgnoreCase));
        }
    }
}