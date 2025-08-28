using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Components;
using CoreMvvmLib.Core.Messenger;
using System.Windows;
using wpfCodeCheck.Main.Local.Exceptions;
using wpfCodeCheck.Main.Local.Models;
using wpfCodeCheck.Domain.Enums;
using wpfCodeCheck.Domain.Datas;
using wpfCodeCheck.Domain.Services;
using wpfCodeCheck.Main.Services;

namespace wpfCodeCheck.Main.Local.ViewModels
{
    public partial class FolderCompareViewModel : ViewModelBase
    {
        private List<DirectorySearchResult> _codeInfos = new List<DirectorySearchResult>(2);                   
        
        private readonly IBaseService _baseService;
        private readonly ISettingService _settingService;
        private readonly CompareFactoryService _compareFactoryService;
        private readonly ICompare _compareService;

        public FolderCompareViewModel(IBaseService baseService,
            ISettingService settingService,
            CompareFactoryService compareFactoryService)
        {
            _baseService = baseService;
            _settingService = settingService;
            _compareFactoryService = compareFactoryService;
            _compareService = compareFactoryService.CreateCompareService();

            InputDirectoryPath = _settingService.GeneralSetting?.InputPath ?? string.Empty;
            OutputDirectoryPath = _settingService.GeneralSetting?.OutputPath ?? string.Empty;

            WeakReferenceMessenger.Default.Register<FolderCompareViewModel, DirectorySearchResult>(this, OnReceiveCodeInfos);
        }  
        [Property]
        private string? _inputDirectoryPath;
        [Property]
        private string? _outputDirectoryPath;
        [AsyncRelayCommand]
        private async Task Compare()
        {
            if (_codeInfos.Count != 2)
            {
                throw new InsufficientDataException($"파일 데이터가 부족 합니다.");
            }
            var inputData = _codeInfos.Where(p => p.type == EFolderListType.INPUT).FirstOrDefault();
            var outputData = _codeInfos.Where(p => p.type == EFolderListType.OUTPUT).FirstOrDefault();

            // 비교를 위해 평면화된 파일 리스트 생성
            var inputItems = inputData.fileDatas.Flatten()
                                               .Where(f => f.FileType == EFileType.SOURCECODE)
                                               .ToList();
            var outputItems = outputData.fileDatas.Flatten()
                                                 .Where(f => f.FileType == EFileType.SOURCECODE)
                                                 .ToList();

            var compareResult = await _compareService.CompareModelCollections(inputItems, outputItems);            
            
            // 비교 결과를 원본 계층구조에 반영
            UpdateComparisonResults(inputData.fileDatas, inputItems);
            UpdateComparisonResults(outputData.fileDatas, outputItems);
            
            // 뷰 바인딩을 위해 원본 계층구조 유지 (업데이트된 IsComparison 포함)
            _baseService.SetFolderTypeDictionaryFiles(EFolderListType.INPUT, inputData.fileDatas);
            _baseService.SetFolderTypeDictionaryFiles(EFolderListType.OUTPUT, outputData.fileDatas);

            _baseService.SetDirectoryCompareReuslt(compareResult);
            //var groupedByProjectName = _code2
            //                .GroupBy(codeInfo => codeInfo.ProjectName)
            //                .Select(group => new
            //                {
            //                    ProjectName = group.Key,
            //                    CodeInfos = group.ToList()
            //                });

            //WeakReferenceMessenger.Default.Send<CodeDiffReulstModel, ComparisonResultsViewModel>(_codeCompareModel);
            WeakReferenceMessenger.Default.Send<EMainViewType>(EMainViewType.EXPORT_EXCEL);
        }
        [RelayCommand]
        private void Export()
        {            
            _compareService.CreateCompareOutputInfo();
            MessageBox.Show("완료");
        }
        [RelayCommand]
        private void Clear()
        {
            WeakReferenceMessenger.Default.Send<EFolderCompareList, FolderListViewModel>(EFolderCompareList.CLEAR);
        }
        private DiffReulstModel<T> GetCodeCompareModels<T>(IEnumerable<CodeInfoModel> codeInfos)
        {
            var diffFileModel = new DiffReulstModel<T>();
            List<string> classFile = new List<string>();
            List<string> classFilePath = new List<string>();
            foreach (var item in codeInfos)
            {
                if (item.IsComparison == false)
                {
                    classFile.Add(item.FileName);
                    classFilePath.Add(item.FilePath);
                }
            }
            return diffFileModel;
        }
        private void OnReceiveCodeInfos(FolderCompareViewModel model, DirectorySearchResult directorySearchResult)
        {
            if (_codeInfos.Count >= 2)
            {
                _codeInfos.Clear();
            }            
            _codeInfos.Add(directorySearchResult);
        }

        private void UpdateComparisonResults(IList<FileCompareModel> hierarchyItems, IList<FileCompareModel> flatItems)
        {
            var flatLookup = flatItems.ToDictionary(f => f.FilePath, f => f.IsComparison);
            
            foreach (var item in hierarchyItems.Flatten())
            {
                if (item.FileType == EFileType.SOURCECODE && flatLookup.ContainsKey(item.FilePath))
                {
                    item.IsComparison = flatLookup[item.FilePath];
                }
            }
        }
    
        //private bool AddCodeCompreResult(CompareEntity customCodeComparer)
        //{
        //    if(customCodeComparer is null)
        //         return false;
        //    if (_codeCompareModel.CompareResults.Contains(customCodeComparer) == false)
        //    {
        //        _codeCompareModel.CompareResults.Add(customCodeComparer);
        //    }
        //    return true;
        //}
    }
}
