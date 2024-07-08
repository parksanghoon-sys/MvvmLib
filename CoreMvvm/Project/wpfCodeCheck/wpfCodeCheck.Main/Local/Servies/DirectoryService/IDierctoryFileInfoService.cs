using wpfCodeCheck.Shared.Local.Models;

namespace wpfCodeCheck.Main.Local.Servies.DirectoryService
{
    public interface IDierctoryFileInfoService
    {
        Task<List<CodeInfo>>GetDirectoryCodeFileInfosAsync(string path);
    }
}
