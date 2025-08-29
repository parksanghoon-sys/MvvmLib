using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Components;
using CoreMvvmLib.WPF.Components;
using CoreMvvmLib.Core.Services.DialogService;
using wpfCodeCheck.Component.UI.Views;
using CoreMvvmLib.Core.Messenger;
using wpfCodeCheck.Domain.Services;
using wpfCodeCheck.Domain.Enums;
using System.Windows;
using System.ComponentModel;
using wpfCodeCheck.Main.Services;
using wpfCodeCheck.Domain.Models;

namespace wpfCodeCheck.Main.Local.ViewModels
{
    public partial class FolderListViewModel : ViewModelBase, IDisposable
    {        
        private readonly IDirectoryCompare _dierctoryFileInfoService;
        private readonly CompareFactoryService _compareFactoryService;
        private readonly IDialogService _dialogService;
        private readonly IBaseService _baseService;

        public FolderListViewModel(CompareFactoryService compareFactoryService, 
            IDialogService dialogService,
            IBaseService baseService)
        {                       
            _compareFactoryService = compareFactoryService;
            _dialogService = dialogService;
            _baseService = baseService;
            _dierctoryFileInfoService = compareFactoryService.CreateIDirectoryCompareService();

            FileDatas = new();

            WeakReferenceMessenger.Default.Register<FolderListViewModel, EFolderCompareList>(this, OnReceiveClearMessage);            
            _baseService.PropertyChanged += OnPropertyChanged;
        }
    
        private string? _folerPath;

        public string? FolderPath
        {
            get => _folerPath;
            set 
            { 
                SetProperty(ref _folerPath,value);
                OnFolderPathChanged();
            }
        }      
        [Property]
        private EFolderListType _folderLIstType;
        [Property]
        private CustomObservableCollection<FileTreeModel> _fileDatas = new();
        [Property]
        private FileTreeModel _fileData = new();
        private bool _disposedValue;
        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            var baseService = sender as IBaseService;
            if (baseService != null)
            {
                if (e.PropertyName == "FolderTypeDirectoryFiles")
                {
                    List<FileTreeModel> files;
                    if (baseService.FolderTypeDirectoryFiles.TryGetValue(FolderLIstType, out files))
                    {
                        FileDatas.Clear();
                        FileDatas.AddRange(FlattenFileTree(files));                                            
                    }
                }
            }
        }

        private void OnReceiveClearMessage(FolderListViewModel model, EFolderCompareList list)
        {
            if (EFolderCompareList.CLEAR == list)
                this.FileDatas.Clear();
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
                var dd = Application.Current.MainWindow;
                _dialogService.Show(this, typeof(LoadingDialogView), 300, 300);
                var folderInfoList = await _dierctoryFileInfoService.GetDirectoryCodeFileInfosAsync(FolderPath);


                await FileDatas.AddItemsAsync(FlattenFileTree(folderInfoList));
                //folderInfoList.ForEach(item =>  FileDatas.Add(item));
                _dialogService.Close(typeof(LoadingDialogView));
                WeakReferenceMessenger.Default.Send<DirectorySearchResult, FolderCompareViewModel>(new DirectorySearchResult(FolderLIstType, folderInfoList));
            }            
        }
        private int _fileIndex = 1;
        private List<FileTreeModel> FlattenFileTree(List<FileTreeModel> list)
        {
            var flattenedList = new List<FileTreeModel>();
            foreach (var item in list)
            {
                if (!item.IsDirectory)
                {
                    // 파일인 경우만 평탄화된 리스트에 추가
                    flattenedList.Add(item);
                }

                if (item.Children != null && item.Children.Any())
                {
                    flattenedList.AddRange(FlattenFileTree(item.Children));
                }
            }

            return flattenedList;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _baseService.PropertyChanged -= OnPropertyChanged;
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
