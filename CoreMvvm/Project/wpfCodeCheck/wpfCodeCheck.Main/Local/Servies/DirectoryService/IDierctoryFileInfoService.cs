using wpfCodeCheck.Main.Local.Models;

namespace wpfCodeCheck.Main.Local.Servies.DirectoryService
{
    public interface IDierctoryFileInfoService
    {
        IList<CodeInfo> GetDirectoryCodeFileInfos(string path);
    }
}
