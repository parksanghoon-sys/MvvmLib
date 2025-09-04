using CoreMvvmLib.Core.Messenger;
using CoreMvvmLib.Core.Services.DialogService;
using System.IO;
using wpfCodeCheck.Component.UI.Views;
using wpfCodeCheck.Domain.Enums;
using wpfCodeCheck.Domain.Models;
using wpfCodeCheck.Domain.Services.Interfaces;
using wpfCodeCheck.Main.Local.Models;
using wpfCodeCheck.Main.Local.Services.DirectoryService;
using wpfCodeCheck.Main.Local.Services.DirectoryService.Interfaces;

namespace wpfCodeCheck.Main.Local.Servies
{
    public interface IDirectoryExplorerService
    {
        /// <summary>
        /// 디렉토리를 탐색
        /// </summary>
        /// <param name="directoryPath">디렉토리 경로</param>
        /// <returns>파일 정보</returns>
        Task<List<FileTreeModel>> ExploreDirectoryAsync(string directoryPath);
        /// <summary>
        /// FileTreeModel 의 모델을 평탄화하여 1차원 FileInfoDto로 변경
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        IEnumerable<FileInfoDto> FlattenAndConvert(List<FileTreeModel> list);
    }
    /// <summary>
    /// 개선된 디렉토리 탐색 통합 서비스    
    /// </summary>
    public class DirectoryExplorerService : IDirectoryExplorerService
    {
        private readonly DirectoryScannerFactory _scannerFactory;
        private readonly ISettingService _settingService;
        private readonly IDialogService _dialogService;

        /// <summary>
        /// 디렉토리 스캔 진행률 이벤트
        /// </summary>
        public event Action<int, string>? ProgressChanged;

        /// <summary>
        /// DirectoryExplorerService 생성자
        /// </summary>
        /// <param name="fileCheckSum">파일 체크섬 서비스</param>
        /// <param name="settingService">설정 서비스</param>
        public DirectoryExplorerService(IFileCheckSum fileCheckSum,
            ISettingService settingService,
            IDialogService dialogService)
        {
            _scannerFactory = new DirectoryScannerFactory(fileCheckSum);
            _settingService = settingService;
            _dialogService = dialogService;
        }

        /// <summary>
        /// 비동기적으로 디렉토리를 탐색하여 파일 트리 모델을 생성합니다.
        /// </summary>
        /// <param name="directoryPath">탐색할 디렉토리 경로</param>
        /// <returns>탐색된 파일과 디렉토리의 트리 구조</returns>
        public async Task<List<FileTreeModel>> ExploreDirectoryAsync(string directoryPath)
        {
            // 기본값으로 FILE 타입 사용 (모든 파일 스캔)
            var compareType = _settingService.GeneralSetting?.CompareType ?? ECompareType.FILE;

            // 디버깅 정보
            ProgressChanged?.Invoke(-1, $"Using scanner type: {compareType}");
            ProgressChanged?.Invoke(-1, $"Scanner description: {_scannerFactory.GetScannerDescription(compareType)}");

            var scanner = _scannerFactory.CreateScanner(compareType);

            // 진행률 이벤트 구독
            scanner.ProgressChanged += OnScannerProgressChanged;

            try
            {
                ProgressChanged?.Invoke(-1, $"Starting scan of directory: {directoryPath}");
                ProgressChanged?.Invoke(-1, $"Directory exists: {Directory.Exists(directoryPath)}");

                var result = await scanner.ScanDirectoryAsync(directoryPath);

                ProgressChanged?.Invoke(-1, $"Scan completed. Found {result.Count} top-level items");
                return result;
            }
            finally
            {
                // 이벤트 구독 해제
                scanner.ProgressChanged -= OnScannerProgressChanged;
            }
        }

        /// <summary>
        /// 스캐너 진행률 이벤트 핸들러
        /// </summary>
        private void OnScannerProgressChanged(int percentage, string message)
        {            
            if (_dialogService.Activate(typeof(LoadingDialogView)))
                WeakReferenceMessenger.Default.Send<(int, string), LoadingDialogView>(new(0, message));

        }
        /// <summary>
        /// 파일 인덱스 카운터
        /// </summary>
        private int _fileIndex = 1;

        /// <summary>
        /// 계층구조의 FileTreeModel 목록을 평면적인 FileInfoDto 목록으로 변환합니다.
        /// </summary>
        /// <param name="list">변환할 FileTreeModel 목록</param>
        /// <returns>평면화된 FileInfoDto 목록</returns>
        public IEnumerable<FileInfoDto> FlattenAndConvert(List<FileTreeModel> list)
        {
            //List<FileInfoDto> result = new();            
            var flatList = Flatten(list);

            return flatList
                .Select((item, index) =>
                {
                    item.FileIndex = ++index;
                    return item;
                });
        }
        private IEnumerable<FileInfoDto> Flatten(List<FileTreeModel> list)
        {
            foreach (var item in list)
            {
                if (!item.IsDirectory)
                {
                    yield return new FileInfoDto
                    {
                        Checksum = item.Checksum,
                        FilePath = item.FilePath,
                        FileName = item.FileName,
                        CreateDate = item.CreateDate,
                        LineCount = item.LineCount,
                        IsComparison = item.IsComparison,
                        FileSize = item.FileSize,
                        FileType = item.FileType,
                    };
                }

                if (item.Children?.Any() == true)
                {
                    foreach (var child in Flatten(item.Children))
                        yield return child;
                }
            }

        }
    }
   
}