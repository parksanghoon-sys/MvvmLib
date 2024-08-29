namespace wpfCodeCheck.Domain.Services
{
    public interface IProjectDirectoryCompare<T>
    {
        Task<List<T>> GetDirectoryCodeFileInfosAsync(string path);
    }
}
