namespace wpfCodeCheck.Domain.Services
{
    public interface IDierctoryFileInfoService<T>
    {
        Task<List<T>> GetDirectoryCodeFileInfosAsync(string path);
    }
}
