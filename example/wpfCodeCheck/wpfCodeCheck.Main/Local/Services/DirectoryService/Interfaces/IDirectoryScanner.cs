using wpfCodeCheck.Domain.Models;

namespace wpfCodeCheck.Main.Local.Services.DirectoryService.Interfaces
{
    /// <summary>
    /// 디렉토리 스캔 서비스 인터페이스
    /// </summary>
    public interface IDirectoryScanner
    {
        /// <summary>
        /// 지정된 경로에서 파일 트리를 비동기적으로 생성
        /// </summary>
        Task<List<FileTreeModel>> ScanDirectoryAsync(string path);
        
        /// <summary>
        /// 디렉토리 스캔 진행률 이벤트
        /// </summary>
        event Action<int, string>? ProgressChanged;
    }
}