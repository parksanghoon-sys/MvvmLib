using System.ComponentModel;
using wpfCodeCheck.Domain.Models;

namespace wpfCodeCheck.Domain.Services.Interfaces;

public interface IBaseService : INotifyPropertyChanged
{
    FileTreeModel? InputTree { get; }
    FileTreeModel? OutputTree { get; }
    List<FileTreeModel> InputFiles { get; }
    List<FileTreeModel> OutputFiles { get; }
    List<FileTreeModel> DifferenceFiles { get; }

    Task LoadInputDirectoryAsync(string inputPath);
    Task LoadOutputDirectoryAsync(string outputPath);
    Task CompareDirectoriesAsync();

    // Legacy methods for backward compatibility
    void SetInputFiles(List<FileTreeModel> files);
    void SetOutputFiles(List<FileTreeModel> files);
    void SetDifferenceFiles(List<FileTreeModel> files);
}
