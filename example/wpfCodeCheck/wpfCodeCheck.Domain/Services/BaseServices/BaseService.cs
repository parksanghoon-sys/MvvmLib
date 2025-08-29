using Microsoft.CodeAnalysis;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using wpfCodeCheck.Domain.Models.Base;
using wpfCodeCheck.Domain.Models;
using wpfCodeCheck.Domain.Enums;

namespace wpfCodeCheck.Domain.Services
{
    public class BaseService : BaseModel, IBaseService        
    {        
        private List<FileTreeModel> _inputFiles = new();
        private List<FileTreeModel> _outputFiles = new();
        private List<FileTreeModel> _differenceFiles = new();

        public List<FileTreeModel> InputFiles
        {
            get => _inputFiles;
            private set => SetProperty(ref _inputFiles, value);
        }

        public List<FileTreeModel> OutputFiles
        {
            get => _outputFiles;
            private set => SetProperty(ref _outputFiles, value);
        }

        public List<FileTreeModel> DifferenceFiles
        {
            get => _differenceFiles;
            private set => SetProperty(ref _differenceFiles, value);
        }

        public void SetInputFiles(List<FileTreeModel> files)
        {
            InputFiles = files;
        }

        public void SetOutputFiles(List<FileTreeModel> files)
        {
            OutputFiles = files;
        }

        public void SetDifferenceFiles(List<FileTreeModel> files)
        {
            DifferenceFiles = files;
        }
     
    }
}
