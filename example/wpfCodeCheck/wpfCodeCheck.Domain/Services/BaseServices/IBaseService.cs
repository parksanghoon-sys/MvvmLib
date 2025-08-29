using System.ComponentModel;
using wpfCodeCheck.Domain.Models;

namespace wpfCodeCheck.Domain.Services
{
    public interface IBaseService : INotifyPropertyChanged
    {
        List<FileTreeModel> InputFiles { get; }
        List<FileTreeModel> OutputFiles { get; }
        List<FileTreeModel> DifferenceFiles { get; }

        void SetInputFiles(List<FileTreeModel> files);
        void SetOutputFiles(List<FileTreeModel> files);
        void SetDifferenceFiles(List<FileTreeModel> files);
    }
}
