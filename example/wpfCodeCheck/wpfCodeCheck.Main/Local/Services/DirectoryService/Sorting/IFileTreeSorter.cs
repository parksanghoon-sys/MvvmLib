using wpfCodeCheck.Domain.Models;

namespace wpfCodeCheck.Main.Local.Services.DirectoryService.Sorting
{
    /// <summary>
    /// 파일 트리 정렬 서비스 인터페이스
    /// </summary>
    public interface IFileTreeSorter
    {
        /// <summary>
        /// 파일 트리를 정렬합니다
        /// </summary>
        List<FileTreeModel> SortFileTree(List<FileTreeModel> files);
        
        /// <summary>
        /// 정렬 전략 이름
        /// </summary>
        string SorterName { get; }
    }
}