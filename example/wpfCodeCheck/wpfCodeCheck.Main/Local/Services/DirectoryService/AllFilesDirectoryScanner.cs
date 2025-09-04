using wpfCodeCheck.Domain.Services.Interfaces;
using wpfCodeCheck.Main.Local.Services.DirectoryService.Strategies;
using wpfCodeCheck.Main.Local.Services.DirectoryService.Sorting;

namespace wpfCodeCheck.Main.Local.Services.DirectoryService
{
    /// <summary>
    /// 모든 파일 디렉토리 스캐너
    /// AllFilesFilterStrategy를 사용하여 모든 파일을 스캔 (시스템 폴더만 제외)
    /// </summary>
    public class AllFilesDirectoryScanner : BaseDirectoryScanner
    {
        public AllFilesDirectoryScanner(IFileCheckSum fileCheckSum) 
            : base(fileCheckSum, new AllFilesFilterStrategy(), new StandardFileTreeSorter())
        {
        }

        public AllFilesDirectoryScanner(IFileCheckSum fileCheckSum, IFileTreeSorter customSorter) 
            : base(fileCheckSum, new AllFilesFilterStrategy(), customSorter)
        {
        }

        public override string GetScannerInfo()
        {
            return $"AllFiles Scanner - {base.GetScannerInfo()}";
        }
    }
}