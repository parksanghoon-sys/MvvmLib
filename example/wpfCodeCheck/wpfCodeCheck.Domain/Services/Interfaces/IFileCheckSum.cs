using wpfCodeCheck.Domain.Models;

namespace wpfCodeCheck.Domain.Services.Interfaces;

public interface IFileCheckSum
{
    Task<string> CalculateChecksumAsync(string filePath);
    Task<List<FileTreeModel>> GetDirectoryFileInfosAsync(string directoryPath);
}