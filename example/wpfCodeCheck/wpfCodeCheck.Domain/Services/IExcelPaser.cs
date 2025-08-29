using wpfCodeCheck.Domain.Models;

namespace wpfCodeCheck.Domain.Services
{
    public interface IExcelPaser
    {
        Task<bool> WriteExcelAync(string fileFullName);
        Task<bool> WriteExcelAync(FileTreeModel inputFile, FileTreeModel outputFile);
    }
}