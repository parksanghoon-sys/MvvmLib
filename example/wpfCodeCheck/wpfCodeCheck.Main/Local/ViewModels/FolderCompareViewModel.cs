using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Components;
using CoreMvvmLib.Core.Messenger;
using System.Windows;
using wpfCodeCheck.Domain.Enums;
using wpfCodeCheck.Domain.Models;
using wpfCodeCheck.Domain.Services.Interfaces;
using wpfCodeCheck.Domain.Services.LogService;
using wpfCodeCheck.Main.Local.Exceptions;
using wpfCodeCheck.Main.Local.Models;
using wpfCodeCheck.Main.Local.Services.CompareService;
using wpfCodeCheck.Main.Local.Servies;

namespace wpfCodeCheck.Main.Local.ViewModels
{
    /// <summary>
    /// 폴더 비교 기능을 담당하는 뷰모델 클래스
    /// 두 디렉토리를 비교하여 차이점을 분석하고 결과를 내보냅니다.
    /// </summary>
    public partial class FolderCompareViewModel : ViewModelBase, IDisposable
    {        
        /// <summary>
        /// 입력 디렉토리의 파일 트리 모델
        /// </summary>
        private List<FileTreeModel> _inputFileTree = new();
        /// <summary>
        /// 출력 디렉토리의 파일 트리 모델
        /// </summary>
        private List<FileTreeModel> _outputFileTree = new();                
        
        private readonly IBaseService _baseService;
        private readonly ISettingService _settingService;
        private readonly ICompareService _compareService;
        private readonly ILoggerService _loggerService;

        /// <summary>
        /// FolderCompareViewModel 생성자
        /// </summary>
        /// <param name="baseService">기본 서비스</param>
        /// <param name="settingService">설정 서비스</param>
        /// <param name="fileCheckSum">파일 체크섬 서비스</param>
        /// <param name="compareService">비교 서비스</param>
        public FolderCompareViewModel(IBaseService baseService,
            ISettingService settingService,            
            IFileCheckSum fileCheckSum,
            ICompareService compareService,
            ILoggerService loggerService)
        {
            _baseService = baseService;
            _settingService = settingService;
            _compareService = compareService;
            _loggerService = loggerService;
            // 진행률 이벤트 구독
            //_directoryExplorerService.ProgressChanged += OnDirectoryExplorerProgressChanged;

            InputDirectoryPath = _settingService.GeneralSetting?.InputPath ?? string.Empty;
            OutputDirectoryPath = _settingService.GeneralSetting?.OutputPath ?? string.Empty;

            WeakReferenceMessenger.Default.Register<FolderCompareViewModel, DirectorySearchResult>(this, OnReceiveCodeInfos);
        }  
        /// <summary>
        /// 비교할 입력 디렉토리 경로
        /// </summary>
        [Property]
        private string? _inputDirectoryPath;
        /// <summary>
        /// 비교할 출력 디렉토리 경로
        /// </summary>
        [Property]
        private string? _outputDirectoryPath;

        /// <summary>
        /// 두 디렉토리를 비동기적으로 비교하는 커맨드
        /// </summary>
        /// <returns>비동기 작업 태스크</returns>
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
                _loggerService.Info($"Compare : Diff TotalCount = {compareResults.Count}");
                WeakReferenceMessenger.Default.Send<EMainViewType>(EMainViewType.EXPORT_EXCEL);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"디렉토리 비교 중 오류 발생: {ex.Message}");
            }
            finally
            {

            }
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

        public void Dispose()
        {
            WeakReferenceMessenger.Default.UnRegister<FolderCompareViewModel, DirectorySearchResult>(this, OnReceiveCodeInfos);
        }
      
    }
}
