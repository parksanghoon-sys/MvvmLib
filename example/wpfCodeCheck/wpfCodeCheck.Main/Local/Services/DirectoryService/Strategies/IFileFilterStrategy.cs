using System.IO;

namespace wpfCodeCheck.Main.Local.Services.DirectoryService.Strategies
{
    /// <summary>
    /// 파일 필터링 전략 인터페이스
    /// </summary>
    public interface IFileFilterStrategy
    {
        /// <summary>
        /// 검색할 파일 패턴들
        /// </summary>
        string[] FilePatterns { get; }
        
        /// <summary>
        /// 제외할 파일명들
        /// </summary>
        string[] ExcludedFileNames { get; }
        
        /// <summary>
        /// 제외할 폴더명들
        /// </summary>
        string[] ExcludedFolders { get; }
        
        /// <summary>
        /// 파일이 허용되는지 확인
        /// </summary>
        bool IsFileAllowed(FileInfo fileInfo);
        
        /// <summary>
        /// 폴더가 허용되는지 확인
        /// </summary>
        bool IsFolderAllowed(DirectoryInfo directoryInfo);
        
        /// <summary>
        /// 필터링 전략의 이름
        /// </summary>
        string StrategyName { get; }
    }
}