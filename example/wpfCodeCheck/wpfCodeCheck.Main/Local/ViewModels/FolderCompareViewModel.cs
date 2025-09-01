using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Components;
using CoreMvvmLib.Core.Messenger;
using System.Windows;
using wpfCodeCheck.Main.Local.Exceptions;
using wpfCodeCheck.Domain.Enums;
using wpfCodeCheck.Domain.Models;
using wpfCodeCheck.Main.Local.Models;
using wpfCodeCheck.Main.Local.Servies;
using wpfCodeCheck.Domain.Services.Interfaces;
using wpfCodeCheck.Main.Local.Services.CompareService;

namespace wpfCodeCheck.Main.Local.ViewModels
{
    public partial class FolderCompareViewModel : ViewModelBase, IDisposable
    {        
        private List<FileTreeModel> _inputFileTree = new();
        private List<FileTreeModel> _outputFileTree = new();
        
        private readonly DirectoryExplorerService _directoryExplorerService;        
        
        private readonly IBaseService _baseService;
        private readonly ISettingService _settingService;
        private readonly ICompareService _compareService;

        public FolderCompareViewModel(IBaseService baseService,
            ISettingService settingService,            
            IFileCheckSum fileCheckSum,
            ICompareService compareService,
            DirectoryExplorerService directoryExplorerService)
        {
            _baseService = baseService;
            _settingService = settingService;
            _compareService = compareService;
            _directoryExplorerService = directoryExplorerService;
            // 진행률 이벤트 구독
            _directoryExplorerService.ProgressChanged += OnDirectoryExplorerProgressChanged;

            InputDirectoryPath = _settingService.GeneralSetting?.InputPath ?? string.Empty;
            OutputDirectoryPath = _settingService.GeneralSetting?.OutputPath ?? string.Empty;

            WeakReferenceMessenger.Default.Register<FolderCompareViewModel, DirectorySearchResult>(this, OnReceiveCodeInfos);
        }  
        [Property]
        private string? _inputDirectoryPath;
        [Property]
        private string? _outputDirectoryPath;
        [Property]
        private int _scanProgress;
        [Property]
        private string _scanStatusMessage = "대기 중...";
        [AsyncRelayCommand]
        private async Task DirectoryCompare()
        {
            try
            {
                if (string.IsNullOrEmpty(InputDirectoryPath) || string.IsNullOrEmpty(OutputDirectoryPath))
                {
                    throw new InsufficientDataException("Input 및 Output 경로를 모두 입력해야 합니다.");
                }

                _inputFileTree = _baseService.GetFolderTypeDictionaryFiles(EFolderListType.INPUT);
                _outputFileTree = _baseService.GetFolderTypeDictionaryFiles(EFolderListType.OUTPUT);
                         

                // 파일 비교 (새로운 FileComparisonService 사용)
                var differences = await _compareService.CompareFileTreesAsync(_inputFileTree, _outputFileTree);

                // BaseService에 결과 저장
                _baseService.SetFolderTypeDictionaryFiles(EFolderListType.INPUT, _inputFileTree);
                _baseService.SetFolderTypeDictionaryFiles(EFolderListType.OUTPUT, _outputFileTree);

                // 차이점을 CompareEntity 형태로 변환하여 저장
                var compareResults = _compareService.ConvertToCompareEntities(differences);
                _baseService.SetDirectoryCompareReuslt(compareResults);
              
                

                WeakReferenceMessenger.Default.Send<EMainViewType>(EMainViewType.EXPORT_EXCEL);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"디렉토리 비교 중 오류 발생: {ex.Message}");
            }
            finally
            {
                ScanProgress = 0;
                ScanStatusMessage = "대기 중...";
            }
        }

        /// <summary>
        /// 디렉토리 탐색 진행률 이벤트 핸들러
        /// </summary>
        private void OnDirectoryExplorerProgressChanged(int percentage, string message)
        {
            ScanProgress = Math.Max(0, Math.Min(100, percentage));
            ScanStatusMessage = message;
        }
        [RelayCommand]
        private void Export()
        {            
            //_compareService.CreateCompareOutputInfo();
            //MessageBox.Show("완료");
        }
        [RelayCommand]
        private void Clear()
        {
            WeakReferenceMessenger.Default.Send<EFolderCompareList, FolderListViewModel>(EFolderCompareList.CLEAR);
        }
        //private DiffReulstModel<T> GetCodeCompareModels<T>(IEnumerable<CodeInfoModel> codeInfos)
        //{
        //    var diffFileModel = new DiffReulstModel<T>();
        //    List<string> classFile = new List<string>();
        //    List<string> classFilePath = new List<string>();
        //    foreach (var item in codeInfos)
        //    {
        //        if (item.IsComparison == false)
        //        {
        //            classFile.Add(item.FileName);
        //            classFilePath.Add(item.FilePath);
        //        }
        //    }
        //    return diffFileModel;
        //}
        private void OnReceiveCodeInfos(FolderCompareViewModel model, DirectorySearchResult directorySearchResult)
        {
            _baseService.SetFolderTypeDictionaryFiles(directorySearchResult.type, directorySearchResult.fileDatas);
        }

        private void UpdateComparisonResults(IList<FileTreeModel> hierarchyItems, IList<FileTreeModel> flatItems)
        {
            var flatLookup = flatItems.ToDictionary(f => f.FilePath, f => f.IsComparison);
            
            foreach (var item in hierarchyItems.SelectMany(h => h.GetAllDescendants()))
            {
                if (!item.IsDirectory && flatLookup.ContainsKey(item.FilePath))
                {
                    item.IsComparison = flatLookup[item.FilePath];
                }
            }
        }

        public void Dispose()
        {
            if (_directoryExplorerService != null)
            {
                _directoryExplorerService.ProgressChanged -= OnDirectoryExplorerProgressChanged;
            }

            WeakReferenceMessenger.Default.UnRegister<FolderCompareViewModel, DirectorySearchResult>(this, OnReceiveCodeInfos);
        }
      
    }
}
