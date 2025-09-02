using CoreMvvmLib.Core.Messenger;
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
        List<FileInfoDto> FlattenAndConvert(List<FileTreeModel> list);
    }
    /// <summary>
    /// 개선된 디렉토리 탐색 통합 서비스    
    /// </summary>
    public class DirectoryExplorerService : IDirectoryExplorerService
    {
        private readonly DirectoryScannerFactory _scannerFactory;
        private readonly ISettingService _settingService;

        // 진행률 이벤트
        public event Action<int, string>? ProgressChanged;

        public DirectoryExplorerService(IFileCheckSum fileCheckSum, ISettingService settingService)
        {
            _scannerFactory = new DirectoryScannerFactory(fileCheckSum);
            _settingService = settingService;
        }

        /// <summary>
        /// 지정된 타입에 따라 디렉토리 탐색 (개선된 버전)
        /// </summary>
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
            int scanProgress = Math.Max(0, Math.Min(100, percentage));
            //WeakReferenceMessenger.Default.Send<(int, string), LoadingDialogView>(new(scanProgress, message));            
        }
        private int _fileIndex = 1;
        public List<FileInfoDto> FlattenAndConvert(List<FileTreeModel> list)
        {
            List<FileInfoDto> result = new();            
            foreach (var item in list)
            {
                if (!item.IsDirectory)
                {
                    // 파일인 경우만 CodeInfoModel로 변환하여 추가
                    result.Add(new FileInfoDto()
                    {
                        Checksum = item.Checksum,
                        FilePath = item.FilePath,
                        FileName = item.FileName,
                        CreateDate = item.CreateDate,
                        LineCount = item.LineCount,
                        IsComparison = item.IsComparison,
                        FileSize = item.FileSize,
                        FileIndex = _fileIndex++,
                        FileType = item.FileType,
                    });
                }

                if (item.Children != null && item.Children.Any())
                {
                    result.AddRange(FlattenAndConvert(item.Children));
                }
            }
            return result;
        }

    }

   
}