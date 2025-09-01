using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Components;
using CoreMvvmLib.Core.Messenger;
using CoreMvvmLib.Core.Services.DialogService;
using CoreMvvmLib.WPF.Components;
using System.ComponentModel;
using System.Windows;
using wpfCodeCheck.Component.UI.Views;
using wpfCodeCheck.Domain.Enums;
using wpfCodeCheck.Domain.Models;
using wpfCodeCheck.Domain.Services.Interfaces;
using wpfCodeCheck.Main.Local.Models;
using wpfCodeCheck.Main.Local.Servies;
using wpfCodeCheck.Main.Local.Services.FileDescription;

namespace wpfCodeCheck.Main.Local.ViewModels
{
    public partial class FolderListViewModel : ViewModelBase, IDisposable
    {                        
        private readonly IDialogService _dialogService;
        private readonly IBaseService _baseService;
        private readonly DirectoryExplorerService _directoryExplorerService;
        private readonly IFileDescriptionService _fileDescriptionService;

        public FolderListViewModel( 
            IDialogService dialogService,
            IBaseService baseService,
            DirectoryExplorerService directoryExplorerService,
            IFileDescriptionService fileDescriptionService)
        {                                   
            _dialogService = dialogService;
            _baseService = baseService;
            _directoryExplorerService = directoryExplorerService;
            _fileDescriptionService = fileDescriptionService;
            FileDatas = new();

            WeakReferenceMessenger.Default.Register<FolderListViewModel, EFolderCompareList>(this, OnReceiveClearMessage);
            WeakReferenceMessenger.Default.Register<FolderListViewModel, ComparisonResultMessage>(this, OnReceiveComparisonResult);
            _baseService.PropertyChanged += OnPropertyChanged;
        }
        
        private string? _folderPath;

        public string? FolderPath
        {
            get => _folderPath;
            set 
            { 
                SetProperty(ref _folderPath,value);
                OnFolderPathChanged();
            }
        }      
        [Property]
        private EFolderListType _folderListType;
        [Property]
        private CustomObservableCollection<FileInfoDto> _fileDatas = new();
        [Property]
        private FileInfoDto _fileData = new();
        private bool _disposedValue;
        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            var baseService = sender as IBaseService;
            if (baseService != null)
            {
                if (e.PropertyName == "FolderTypeDictionaryFiles")
                {
                    List<FileTreeModel> files = baseService.GetFolderTypeDictionaryFiles(FolderListType);
                    if (files.Count > 0)
                    {
                        FileDatas.Clear();
                        FileDatas.AddRange(_directoryExplorerService.FlattenAndConvert(files));                                            
                    }
                }
            }
        }

        private void OnReceiveClearMessage(FolderListViewModel model, EFolderCompareList list)
        {
            if (EFolderCompareList.CLEAR == list)
                this.FileDatas.Clear();
        }

        /// <summary>
        /// 비교 결과를 받아서 IsComparison 값 업데이트
        /// </summary>
        private void OnReceiveComparisonResult(FolderListViewModel model, ComparisonResultMessage comparisonResult)
        {
            if (FileDatas == null || !FileDatas.Any()) return;

            // FileTreeModel의 IsComparison 값을 FileInfoDto에 반영
            var fileTreeLookup = comparisonResult.ComparedFiles
                .SelectMany(tree => FlattenFileTree(tree))
                .Where(f => !f.IsDirectory)
                .ToDictionary(f => f.FilePath, f => f.IsComparison);

            foreach (var fileData in FileDatas)
            {
                if (fileTreeLookup.TryGetValue(fileData.FilePath, out var isComparison))
                {
                    fileData.IsComparison = isComparison;
                }
            }
        }

        /// <summary>
        /// FileTreeModel을 평면화하여 모든 파일 추출
        /// </summary>
        private IEnumerable<FileTreeModel> FlattenFileTree(FileTreeModel node)
        {
            if (!node.IsDirectory)
            {
                yield return node;
            }

            foreach (var child in node.Children)
            {
                foreach (var descendant in FlattenFileTree(child))
                {
                    yield return descendant;
                }
            }
        }
        private async void OnFolderPathChanged()
        {
            if (string.IsNullOrEmpty(FolderPath) == false)
            {
                FileDatas.Clear();
                await GetDirectoryFilesInfoAsync();                
            }
        }
        private async Task GetDirectoryFilesInfoAsync()
        {
            if (string.IsNullOrEmpty(FolderPath) == false)
            {
                Console.WriteLine($"[DEBUG] Starting directory scan: {FolderPath}");
                Console.WriteLine($"[DEBUG] FolderListType: {FolderListType}");
                
                _dialogService.Show(this, typeof(LoadingDialogView), 300, 300);
                
                // 진행률 이벤트 구독 (디버깅용)
                _directoryExplorerService.ProgressChanged += OnExplorerProgress;
                
                try
                {
                    var folderInfoList = await _directoryExplorerService.ExploreDirectoryAsync(FolderPath);
                    Console.WriteLine($"[DEBUG] Scan completed. Found {folderInfoList.Count} items");
                    
                    _baseService.SetFolderTypeDictionaryFiles(FolderListType, folderInfoList);
                    FileDatas.Clear();

                    var flattenedItems = _directoryExplorerService.FlattenAndConvert(folderInfoList);
                    Console.WriteLine($"[DEBUG] Flattened to {flattenedItems.Count} files");
                    
                    await FileDatas.AddItemsAsync(flattenedItems);
                    Console.WriteLine($"[DEBUG] Added {FileDatas.Count} items to UI");
                    
                    WeakReferenceMessenger.Default.Send<DirectorySearchResult, FolderCompareViewModel>(new DirectorySearchResult(FolderListType, folderInfoList));
                }
                finally
                {
                    _directoryExplorerService.ProgressChanged -= OnExplorerProgress;
                    _dialogService.Close(typeof(LoadingDialogView));
                }
            }            
        }
        
        private void OnExplorerProgress(int percentage, string message)
        {
            Console.WriteLine($"[DEBUG] {percentage}%: {message}");
        }       

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _baseService.PropertyChanged -= OnPropertyChanged;
                    WeakReferenceMessenger.Default.UnRegister<FolderListViewModel, EFolderCompareList>(this, OnReceiveClearMessage);
                    WeakReferenceMessenger.Default.UnRegister<FolderListViewModel, ComparisonResultMessage>(this, OnReceiveComparisonResult);
                    // TODO: 관리형 상태(관리형 개체)를 삭제합니다.
                }

                // TODO: 비관리형 리소스(비관리형 개체)를 해제하고 종료자를 재정의합니다.
                // TODO: 큰 필드를 null로 설정합니다.
                _disposedValue = true;
            }
        }

        // // TODO: 비관리형 리소스를 해제하는 코드가 'Dispose(bool disposing)'에 포함된 경우에만 종료자를 재정의합니다.
        // ~FolderListViewModel()
        // {
        //     // 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
   
}
