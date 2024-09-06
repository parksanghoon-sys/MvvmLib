using wpfCodeCheck.Domain.Datas;

namespace wpfCodeCheck.Domain.Services.DirectoryServices
{
    public interface IDirectoryCompare
    {
        Task<List<FileCompareModel>> GetDirectoryCodeFileInfosAsync(string path);
    }
}
