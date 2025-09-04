using wpfCodeCheck.Domain.Enums;
using wpfCodeCheck.Domain.Services.Interfaces;
using wpfCodeCheck.Main.Local.Services.DirectoryService.Interfaces;
using wpfCodeCheck.Main.Local.Services.DirectoryService.Sorting;

namespace wpfCodeCheck.Main.Local.Services.DirectoryService
{
    /// <summary>
    /// 디렉토리 스캐너 팩토리
    /// Factory 패턴을 사용하여 적절한 스캐너 인스턴스 생성
    /// </summary>
    public class DirectoryScannerFactory
    {
        private readonly IFileCheckSum _fileCheckSum;

        public DirectoryScannerFactory(IFileCheckSum fileCheckSum)
        {
            _fileCheckSum = fileCheckSum ?? throw new ArgumentNullException(nameof(fileCheckSum));
        }

        /// <summary>
        /// 비교 타입에 따라 적절한 디렉토리 스캐너 생성
        /// </summary>
        public IDirectoryScanner CreateScanner(ECompareType compareType)
        {
            return compareType switch
            {
                ECompareType.SOURCECODE => new SourceCodeDirectoryScanner(_fileCheckSum),
                ECompareType.FILE => new AllFilesDirectoryScanner(_fileCheckSum),
                _ => throw new ArgumentException($"Unsupported compare type: {compareType}", nameof(compareType))
            };
        }

        /// <summary>
        /// 비교 타입과 사용자 정의 정렬기로 스캐너 생성
        /// </summary>
        public IDirectoryScanner CreateScanner(ECompareType compareType, IFileTreeSorter customSorter)
        {
            return compareType switch
            {
                ECompareType.SOURCECODE => new SourceCodeDirectoryScanner(_fileCheckSum, customSorter),
                ECompareType.FILE => new AllFilesDirectoryScanner(_fileCheckSum, customSorter),
                _ => throw new ArgumentException($"Unsupported compare type: {compareType}", nameof(compareType))
            };
        }

        /// <summary>
        /// 사용 가능한 모든 스캐너 타입 반환
        /// </summary>
        public IEnumerable<ECompareType> GetSupportedScannerTypes()
        {
            return new[] { ECompareType.SOURCECODE, ECompareType.FILE };
        }

        /// <summary>
        /// 스캐너 타입별 설명 반환
        /// </summary>
        public string GetScannerDescription(ECompareType compareType)
        {
            return compareType switch
            {
                ECompareType.SOURCECODE => "소스코드 파일만 스캔 (*.cs, *.cpp, *.h, *.xaml 등)",
                ECompareType.FILE => "모든 파일 스캔 (시스템 폴더 제외)",
                _ => "알 수 없는 스캐너 타입"
            };
        }
    }
}