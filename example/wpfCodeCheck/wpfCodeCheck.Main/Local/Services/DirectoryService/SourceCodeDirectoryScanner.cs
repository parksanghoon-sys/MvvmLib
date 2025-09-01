using wpfCodeCheck.Domain.Services.Interfaces;
using wpfCodeCheck.Main.Local.Services.DirectoryService.Strategies;
using wpfCodeCheck.Main.Local.Services.DirectoryService.Sorting;

namespace wpfCodeCheck.Main.Local.Services.DirectoryService
{
    /// <summary>
    /// 소스코드 전용 디렉토리 스캐너
    /// SourceCodeFilterStrategy를 사용하여 소스코드 파일만 스캔
    /// </summary>
    public class SourceCodeDirectoryScanner : BaseDirectoryScanner
    {
        public SourceCodeDirectoryScanner(IFileCheckSum fileCheckSum) 
            : base(fileCheckSum, new SourceCodeFilterStrategy(), new StandardFileTreeSorter())
        {
        }

        public SourceCodeDirectoryScanner(IFileCheckSum fileCheckSum, IFileTreeSorter customSorter) 
            : base(fileCheckSum, new SourceCodeFilterStrategy(), customSorter)
        {
        }

        public override string GetScannerInfo()
        {
            return $"SourceCode Scanner - {base.GetScannerInfo()}";
        }
    }
}