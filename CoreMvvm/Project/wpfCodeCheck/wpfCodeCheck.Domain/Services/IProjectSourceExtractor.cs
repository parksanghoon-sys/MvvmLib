namespace wpfCodeCheck.Domain.Services
{
    public interface IProjectSourceExtractor<T>
    {
        Task<List<T>> GetDirectoryCodeFileInfosAsync(string path);
    }
}
