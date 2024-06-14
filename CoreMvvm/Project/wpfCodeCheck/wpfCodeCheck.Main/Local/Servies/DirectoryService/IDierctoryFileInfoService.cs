using wpfCodeCheck.Main.Local.Models;

namespace wpfCodeCheck.Main.Local.Servies.DirectoryService
{
    public interface IDierctoryFileInfoService
    {
        IEnumerable<CodeInfo> GetDirectoryCodeFileInfos(string path);
    }
