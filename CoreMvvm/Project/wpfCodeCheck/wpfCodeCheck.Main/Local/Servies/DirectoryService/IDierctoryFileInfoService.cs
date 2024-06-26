using wpfCodeCheck.Main.Local.Models;

namespace wpfCodeCheck.Main.Local.Servies.DirectoryService
{
    public interface IDierctoryFileInfoService
    {
        Task<IList<CodeInfo>>GetDirectoryCodeFileInfosAsync(string path);
    }
}
