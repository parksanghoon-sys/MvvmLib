using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Components;
using CoreMvvmLib.WPF.Components;
using CoreMvvmLib.Core.Services.DialogService;
using wpfCodeCheck.Component.UI.Views;
using CoreMvvmLib.Core.Messenger;
using wpfCodeCheck.Main.Local.Models;
using wpfCodeCheck.Domain.Services;
using wpfCodeCheck.Domain.Enums;
using System.Windows;
using wpfCodeCheck.Domain.Datas;
using System.ComponentModel;

namespace wpfCodeCheck.Main.Local.ViewModels
{
    public partial class FolderListViewModel : ViewModelBase, IDisposable
    {        
        private readonly IProjectDirectoryCompare<FileCompareModel> _dierctoryFileInfoService;
        private readonly IDialogService _dialogService;
        private readonly IBaseService _baseService;

        public FolderListViewModel(IProjectDirectoryCompare<FileCompareModel> dierctoryFileInfoService, 
            IDialogService dialogService,
            IBaseService baseService)
        {           
            _dierctoryFileInfoService = dierctoryFileInfoService;
            _dialogService = dialogService;
            _baseService = baseService;
            FileDatas = new();

            WeakReferenceMessenger.Default.Register<FolderListViewModel, EFolderCompareList>(this, OnReceiveClearMessage);            
            _baseService.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            var baseService = sender as IBaseService;
            if (baseService != null)
            {
                if (e.PropertyName == "FolderTypeDirectoryFiles")
                {
                    List<FileCompareModel> files;
                    if(baseService.FolderTypeDirectoryFiles.TryGetValue(FolderLIstType, out files))
                    {
                        FileDatas.Clear();
                        FileDatas.AddRange(FlattenHierarchy(files));
                    }
                }
            }
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
        private CustomObservableCollection<CodeInfoModel> _fileDatas = new();
        [Property]
        private FileItem _fileData = new();
        private bool _disposedValue;

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


                await FileDatas.AddItemsAsync(FlattenHierarchy(folderInfoList));
                //folderInfoList.ForEach(item =>  FileDatas.Add(item));
                _dialogService.Close(typeof(LoadingDialogView));
                WeakReferenceMessenger.Default.Send<DirectorySearchResult, FolderCompareViewModel>(new DirectorySearchResult(FolderLIstType, folderInfoList));
            }            
        }
        private List<CodeInfoModel> FlattenHierarchy(List<FileCompareModel> list)
        {
            var flattenedList = new List<CodeInfoModel>();
            foreach (var item in list)
            {
                if (item.Checksum is not "")
                {
                    flattenedList.Add(new CodeInfoModel()
                    {
                        Checksum = item.Checksum,
                        FilePath = item.FilePath,
                        FileName = item.FileName,
                        CreateDate = item.CreateDate,
                        LineCount = item.LineCount,
                        IsComparison = item.IsComparison,
                        FileSize = item.FileSize,
                     });
                }

                if (item.Children != null)
                {
                    flattenedList.AddRange(FlattenHierarchy(item.Children));
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
