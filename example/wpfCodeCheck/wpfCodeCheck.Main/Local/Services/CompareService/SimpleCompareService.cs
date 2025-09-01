//using wpfCodeCheck.Domain.Models;
//using wpfCodeCheck.Domain.Services;

//namespace wpfCodeCheck.Main.Local.Services.CompareService
//{
//    /// <summary>
//    /// FileTreeService를 사용하는 간소화된 비교 서비스
//    /// </summary>
//    public class SimpleCompareService
//    {
//        private readonly FileTreeService _fileTreeService;

//        public SimpleCompareService()
//        {
//            _fileTreeService = new FileTreeService();
//        }

//        /// <summary>
//        /// 입력 디렉토리에서 파일 트리 생성
//        /// </summary>
//        public async Task<FileTreeModel> GetInputFilesAsync(string inputPath)
//        {
//            return await _fileTreeService.BuildFileTreeAsync(inputPath);
//        }

//        /// <summary>
//        /// 출력 디렉토리에서 파일 트리 생성
//        /// </summary>
//        public async Task<FileTreeModel> GetOutputFilesAsync(string outputPath)
//        {
//            return await _fileTreeService.BuildFileTreeAsync(outputPath);
//        }

//        /// <summary>
//        /// 두 디렉토리를 비교하여 차이점 반환
//        /// </summary>
//        public async Task<List<FileTreeModel>> CompareDirectoriesAsync(string inputPath, string outputPath)
//        {
//            var inputTree = await GetInputFilesAsync(inputPath);
//            var outputTree = await GetOutputFilesAsync(outputPath);

//            return await _fileTreeService.CompareFileTreesAsync(inputTree, outputTree);
//        }

//        /// <summary>
//        /// 파일 트리에서 차이가 있는 파일만 필터링
//        /// </summary>
//        public List<FileTreeModel> GetDifferentFiles(FileTreeModel root)
//        {
//            return root.GetDifferentFiles().ToList();
//        }

//        /// <summary>
//        /// 파일 트리를 평면화하여 모든 파일 반환
//        /// </summary>
//        public List<FileTreeModel> FlattenFileTree(FileTreeModel root)
//        {
//            var result = new List<FileTreeModel>();
//            FlattenRecursive(root, result);
//            return result;
//        }

//        private void FlattenRecursive(FileTreeModel node, List<FileTreeModel> result)
//        {
//            if (!node.IsDirectory)
//            {
//                result.Add(node);
//            }

//            foreach (var child in node.Children)
//            {
//                FlattenRecursive(child, result);
//            }
//        }
//    }
//}