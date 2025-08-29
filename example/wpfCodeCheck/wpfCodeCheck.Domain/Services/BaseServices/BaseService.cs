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
        private readonly FileTreeService _fileTreeService;

        private FileTreeModel? _inputTree;
        private FileTreeModel? _outputTree;
        private List<FileTreeModel> _inputFiles = new();
        private List<FileTreeModel> _outputFiles = new();
        private List<FileTreeModel> _differenceFiles = new();

        public BaseService()
        {
            _fileTreeService = new FileTreeService();
        }

        public FileTreeModel? InputTree
        {
            get => _inputTree;
            private set => SetProperty(ref _inputTree, value);
        }

        public FileTreeModel? OutputTree
        {
            get => _outputTree;
            private set => SetProperty(ref _outputTree, value);
        }

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

        public async Task LoadInputDirectoryAsync(string inputPath)
        {
            InputTree = await _fileTreeService.BuildFileTreeAsync(inputPath);
            InputFiles = InputTree.GetAllDescendants().Where(f => !f.IsDirectory).ToList();
        }

        public async Task LoadOutputDirectoryAsync(string outputPath)
        {
            OutputTree = await _fileTreeService.BuildFileTreeAsync(outputPath);
            OutputFiles = OutputTree.GetAllDescendants().Where(f => !f.IsDirectory).ToList();
        }

        public async Task CompareDirectoriesAsync()
        {
            if (InputTree == null || OutputTree == null)
                throw new InvalidOperationException("Both input and output directories must be loaded first.");

            DifferenceFiles = await _fileTreeService.CompareFileTreesAsync(InputTree, OutputTree);
        }

        // Legacy methods for backward compatibility
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
