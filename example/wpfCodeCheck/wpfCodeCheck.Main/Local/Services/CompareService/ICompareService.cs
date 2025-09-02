using wpfCodeCheck.Domain.Models;


namespace wpfCodeCheck.Main.Local.Services.CompareService
{
    /// <summary>
    /// 파일 트리 비교 기능을 정의하는 인터페이스
    /// 두 파일 트리를 비교하여 차이점을 찾고 결과를 변환하는 기능을 제공합니다.
    /// </summary>
    public interface ICompareService
    {
        /// <summary>
        /// 비동기적으로 두 파일 트리를 비교하여 차이점을 찾습니다.
        /// </summary>
        /// <param name="inputTree">입력 파일 트리</param>
        /// <param name="outputTree">출력 파일 트리</param>
        /// <returns>차이점이 있는 파일 목록</returns>
        Task<List<FileTreeModel>> CompareFileTreesAsync(IList<FileTreeModel> inputTree, IList<FileTreeModel> outputTree);
        /// <summary>
        /// 파일 트리 모델 목록을 비교 엔티티 목록으로 변환합니다.
        /// </summary>
        /// <param name="differences">변환할 파일 트리 모델 목록</param>
        /// <returns>변환된 비교 엔티티 목록</returns>
        List<CompareEntity> ConvertToCompareEntities(List<FileTreeModel> differences);
    }
}
