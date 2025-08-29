using CompareEngine;
using System.IO;
using wpfCodeCheck.Domain.Models;
using wpfCodeCheck.Domain.Services;

namespace wpfCodeCheck.Main.Local.Services.CompareService
{
    /// <summary>
    /// 코드 파일 비교를 위한 서비스 (FileTreeService 기반)
    /// </summary>
    public class CodeCompareService
    {
        private readonly FileTreeService _fileTreeService;

        public CodeCompareService()
        {
            _fileTreeService = new FileTreeService();
        }

        /// <summary>
        /// 두 디렉토리를 비교하여 차이점을 반환
        /// </summary>
        public async Task<List<FileTreeModel>> CompareDirectoriesAsync(string inputPath, string outputPath)
        {
            var inputTree = await _fileTreeService.BuildFileTreeAsync(inputPath);
            var outputTree = await _fileTreeService.BuildFileTreeAsync(outputPath);

            return await _fileTreeService.CompareFileTreesAsync(inputTree, outputTree);
        }

        /// <summary>
        /// 두 개별 파일의 상세 차이점 비교 (Beyond Compare 스타일)
        /// </summary>
        public async Task<CompareResult> CompareFilesDetailAsync(FileTreeModel file1, FileTreeModel file2)
        {
            if (file1.FileSize == null && file2.FileSize == null)
                return new CompareResult { HasDifferences = false };

            CompareText sourceText;
            CompareText destinationText;

            // 첫 번째 파일 처리
            if (File.Exists(file1.FilePath) && !Directory.Exists(file1.FilePath))
                sourceText = new CompareText(file1.FilePath);
            else
                sourceText = new CompareText();

            // 두 번째 파일 처리
            if (File.Exists(file2.FilePath) && !Directory.Exists(file2.FilePath))
                destinationText = new CompareText(file2.FilePath);
            else
                destinationText = new CompareText();

            // CompareEngine을 사용하여 비교
            var compareEngine = new CompareEngine.CompareEngine();
            compareEngine.StartDiff(sourceText, destinationText);

            var resultLines = compareEngine.DiffResult();

            return new CompareResult
            {
                HasDifferences = resultLines.Count > 0,
                SourceText = sourceText,
                DestinationText = destinationText,
                DiffResults = ConvertArrayListToList<CompareResultSpan>(resultLines),
                File1 = file1,
                File2 = file2
            };
        }

        /// <summary>
        /// ArrayList을 Generic List로 변환
        /// </summary>
        private List<T> ConvertArrayListToList<T>(ArrayList list)
        {
            var result = new List<T>();
            foreach (var item in list)
            {
                if (item is T typedItem)
                {
                    result.Add(typedItem);
                }
            }
            return result;
        }
    }

    /// <summary>
    /// 파일 비교 결과
    /// </summary>
    //public class CompareResult
    //{
    //    public bool HasDifferences { get; set; }
    //    public CompareText? SourceText { get; set; }
    //    public CompareText? DestinationText { get; set; }
    //    public List<CompareResultSpan> DiffResults { get; set; } = new List<CompareResultSpan>();
    //    public FileTreeModel? File1 { get; set; }
    //    public FileTreeModel? File2 { get; set; }
    //}
}