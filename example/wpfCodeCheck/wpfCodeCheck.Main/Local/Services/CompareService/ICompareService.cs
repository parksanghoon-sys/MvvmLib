using wpfCodeCheck.Domain.Models;


namespace wpfCodeCheck.Main.Local.Services.CompareService
{
    public interface ICompareService
    {
        Task<List<FileTreeModel>> CompareFileTreesAsync(IList<FileTreeModel> inputTree, IList<FileTreeModel> outputTree);
        List<CompareEntity> ConvertToCompareEntities(List<FileTreeModel> differences);
    }
}
