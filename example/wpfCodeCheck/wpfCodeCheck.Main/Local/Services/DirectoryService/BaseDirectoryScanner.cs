using wpfCodeCheck.Domain.Models;
using wpfCodeCheck.Domain.Services.Interfaces;
using wpfCodeCheck.Main.Local.Services.DirectoryService.Interfaces;
using wpfCodeCheck.Main.Local.Services.DirectoryService.Strategies;
using wpfCodeCheck.Main.Local.Services.DirectoryService.Sorting;
using System.IO;

namespace wpfCodeCheck.Main.Local.Services.DirectoryService
{
    /// <summary>
    /// 디렉토리 스캐너 기본 클래스
    /// Template Method 패턴을 사용하여 공통 로직을 정의
    /// </summary>
    public abstract class BaseDirectoryScanner : IDirectoryScanner
    {
        protected readonly IFileCheckSum _fileCheckSum;
        protected readonly IFileFilterStrategy _filterStrategy;
        protected readonly IFileTreeSorter _sorter;                
        private int _processedItems = 0;

        public event Action<int, string>? ProgressChanged;

        protected BaseDirectoryScanner(
            IFileCheckSum fileCheckSum, 
            IFileFilterStrategy filterStrategy,
            IFileTreeSorter sorter)
        {
            _fileCheckSum = fileCheckSum ?? throw new ArgumentNullException(nameof(fileCheckSum));
            _filterStrategy = filterStrategy ?? throw new ArgumentNullException(nameof(filterStrategy));
            _sorter = sorter ?? throw new ArgumentNullException(nameof(sorter));
        }

        public virtual async Task<List<FileTreeModel>> ScanDirectoryAsync(string path)
        {
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"Directory not found: {path}");
            
            _processedItems = 0;
            
            ReportProgress(0, $"Starting scan: {path}");

            var result = new List<FileTreeModel>();
            
            await Task.Run(async () =>
            {
                await ScanDirectoryRecursivelyAsync(path, result, 0);
            });

            // 정렬 적용
            result = _sorter.SortFileTree(result);
            
            ReportProgress(100, "Scan completed");
            return result;
        }

        /// <summary>
        /// 재귀적으로 디렉토리를 스캔하는 템플릿 메서드
        /// </summary>
        protected virtual async Task ScanDirectoryRecursivelyAsync(string currentPath, List<FileTreeModel> result, int depth)
        {
            try
            {
                var directoryInfo = new DirectoryInfo(currentPath);
                
                // 폴더 필터링 확인
                if (!_filterStrategy.IsFolderAllowed(directoryInfo))
                    return;

                // 하위 디렉토리 처리
                await ProcessSubDirectoriesAsync(currentPath, result, depth);

                // 파일 처리
                await ProcessFilesAsync(currentPath, result, depth);
            }
            catch (Exception ex)
            {
                ReportProgress(-1, $"Error processing {currentPath}: {ex.Message}");
            }
        }

        /// <summary>
        /// 하위 디렉토리 처리
        /// </summary>
        protected virtual async Task ProcessSubDirectoriesAsync(string currentPath, List<FileTreeModel> result, int depth)
        {
            var directories = Directory.GetDirectories(currentPath);
            
            foreach (var directory in directories.OrderBy(d => d))
            {
                var dirInfo = new DirectoryInfo(directory);
                
                if (!_filterStrategy.IsFolderAllowed(dirInfo))
                    continue;

                var directoryModel = CreateDirectoryModel(dirInfo, depth);
                result.Add(directoryModel);

                // 재귀 호출
                await ScanDirectoryRecursivelyAsync(directory, directoryModel.Children, depth + 1);
                
                _processedItems++;
                ReportProgress(_processedItems, $"Processing: {dirInfo.Name}");
            }
        }

        /// <summary>
        /// 파일 처리 (병렬 처리 지원)
        /// </summary>
        protected virtual async Task ProcessFilesAsync(string currentPath, List<FileTreeModel> result, int depth)
        {
            var directoryInfo = new DirectoryInfo(currentPath);
            var allFiles = new List<FileInfo>();

            // 디버깅: 디렉토리 정보 출력
            ReportProgress(-1, $"Processing directory: {currentPath}");
            
            // 필터 패턴에 따라 파일 수집
            foreach (var pattern in _filterStrategy.FilePatterns)
            {
                try
                {
                    var rawFiles = directoryInfo.GetFiles(pattern, SearchOption.TopDirectoryOnly);
                    ReportProgress(-1, $"Pattern '{pattern}' found {rawFiles.Length} files in {directoryInfo.Name}");
                    
                    var filteredFiles = rawFiles.Where(f => _filterStrategy.IsFileAllowed(f)).ToList();
                    ReportProgress(-1, $"After filtering: {filteredFiles.Count} files allowed");
                    
                    allFiles.AddRange(filteredFiles);
                }
                catch (Exception ex)
                {
                    ReportProgress(-1, $"Error getting files with pattern {pattern}: {ex.Message}");
                }
            }
            
            ReportProgress(-1, $"Total files to process: {allFiles.Count}");

            // 병렬 처리로 파일 정보 수집
            var lockObject = new object();
            var semaphore = new SemaphoreSlim(Environment.ProcessorCount);

            await Task.WhenAll(allFiles.Select(async file =>
            {
                await semaphore.WaitAsync();
                try
                {
                    var fileModel = await CreateFileModelAsync(file, depth);
                    
                    lock (lockObject)
                    {
                        result.Add(fileModel);
                        _processedItems++;
                        ReportProgress(_processedItems, $"Processing: {file.Name}");
                    }
                }
                finally
                {
                    semaphore.Release();
                }
            }));
        }

        /// <summary>
        /// 디렉토리 모델 생성
        /// </summary>
        protected virtual FileTreeModel CreateDirectoryModel(DirectoryInfo directoryInfo, int depth)
        {
            return new FileTreeModel
            {
                ProjectName = directoryInfo.Name,
                FileName = directoryInfo.Name,
                FilePath = directoryInfo.FullName,
                Depth = depth,
                FileType = Domain.Enums.EFileType.DIRECTORY,
                Children = new List<FileTreeModel>()
            };
        }

        /// <summary>
        /// 파일 모델 비동기 생성 (체크섬 및 라인 수 계산 포함)
        /// </summary>
        protected virtual async Task<FileTreeModel> CreateFileModelAsync(FileInfo fileInfo, int depth)
        {
            var fileModel = new FileTreeModel
            {
                ProjectName = Path.GetFileNameWithoutExtension(fileInfo.FullName),
                FileName = fileInfo.Name,
                FilePath = fileInfo.FullName,
                FileSize = fileInfo.Length,
                Depth = depth,
                CreateDate = fileInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm"),
                FileType = Domain.Enums.EFileType.FILE
            };

            // 파일 내용 분석 (체크섬, 라인 수)
            try
            {
                await AnalyzeFileContentAsync(fileModel);
            }
            catch (Exception ex)
            {
                ReportProgress(-1, $"Error analyzing file {fileInfo.Name}: {ex.Message}");
                // 분석 실패 시 기본값 유지
                fileModel.Checksum = "error";
                fileModel.LineCount = 0;
            }

            return fileModel;
        }

        /// <summary>
        /// 파일 내용 분석 (체크섬 및 라인 수 계산)
        /// </summary>
        protected virtual async Task AnalyzeFileContentAsync(FileTreeModel fileModel)
        {
            using var fileStream = new FileStream(fileModel.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
            
            var buffer = new byte[fileStream.Length];
            await fileStream.ReadAsync(buffer, 0, (int)fileStream.Length);
            
            // 체크섬 계산
            var checkSum = _fileCheckSum.ComputeChecksum(buffer);
            fileModel.Checksum = checkSum.ToString("x8").ToLower();
            
            // 라인 수 계산
            fileStream.Position = 0;
            using var reader = new StreamReader(fileStream);
            int lineCount = 0;
            
            while (!reader.EndOfStream)
            {
                await reader.ReadLineAsync();
                lineCount++;
            }
            
            fileModel.LineCount = lineCount;
        }

        /// <summary>
        /// 진행률 보고
        /// </summary>
        protected virtual void ReportProgress(int percentage, string message)
        {
            ProgressChanged?.Invoke(percentage, message);
        }

        /// <summary>
        /// 스캐너 정보 반환
        /// </summary>
        public virtual string GetScannerInfo()
        {
            return $"Scanner: {GetType().Name}, Filter: {_filterStrategy.StrategyName}, Sorter: {_sorter.SorterName}";
        }
    }
}