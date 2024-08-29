using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Components;
using CoreMvvmLib.Core.Messenger;
using System.Windows;
using wpfCodeCheck.Main.Local.Exceptions;
using wpfCodeCheck.Main.Local.Models;
using wpfCodeCheck.Domain.Enums;
using wpfCodeCheck.Domain.Datas;
using wpfCodeCheck.Domain.Services;
using wpfCodeCheck.Main.Local.Servies.CodeCompare;
using wpfCodeCheck.Domain.Local.Helpers;

namespace wpfCodeCheck.Main.Local.ViewModels
{
    public partial class FolderCompareViewModel : ViewModelBase
    {
        private class SWDetailedItem
        {
            public string 을Content { get; set; } = string.Empty;
            public string 으로Content { get; set; } = string.Empty;

        }
        private List<DirectorySearchResult> _codeInfos = new List<DirectorySearchResult>(2);
        private IList<CodeInfo> _code1 = new List<CodeInfo>();
        private ICollection<CodeInfo> _code2 = new List<CodeInfo>();
        private CodeDiffReulstModel<CustomCodeComparer> _codeCompareModel = new();

        IList<SWDetailedItem> _detailStatementItems = new List<SWDetailedItem>();

        private readonly ICsvHelper _csvHelper;
        private readonly IBaseService<CustomCodeComparer> _baseService;
        private readonly ISettingService _settingService;
        private readonly CodeCompareService _codeCompareService;

        public FolderCompareViewModel(ICsvHelper csvHelper,
            IBaseService<CustomCodeComparer> baseService,
            ISettingService settingService,
            CodeCompareService codeCompareService)
        {
            _csvHelper = csvHelper;
            _baseService = baseService;
            _settingService = settingService;
            _codeCompareService = codeCompareService;
            IsEnableInputDirectoryList = true;
            IsEnableOutputDirectoryList = false;
            InputDirectoryPath = _settingService.GeneralSetting?.InputPath ?? string.Empty;
            OutputDirectoryPath = _settingService.GeneralSetting?.OutputPath ?? string.Empty;

            WeakReferenceMessenger.Default.Register<FolderCompareViewModel, DirectorySearchResult>(this, OnReceiveCodeInfos);
        }
        [Property]
        private bool _isEnableInputDirectoryList;
        [Property]
        private bool _isEnableOutputDirectoryList;
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
            var inputItems = _codeInfos.Where(p => p.type == EFolderListType.INPUT).FirstOrDefault(); ;
            var outputItems = _codeInfos.Where(p => p.type == EFolderListType.OUTPUT).FirstOrDefault();

            await CompareModelCollections(inputItems!.fileDatas, outputItems!.fileDatas);

            _baseService.SetDirectoryCompareReuslt(_codeCompareModel);
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

            _csvHelper.CreateCSVFile<CodeInfo>(_code2, "CompareProject");
            _csvHelper.CreateCSVFile<SWDetailedItem>(_detailStatementItems, "Detail");


            MessageBox.Show("완료");
        }
        [RelayCommand]
        private void Clear()
        {
            WeakReferenceMessenger.Default.Send<EFolderCompareList, FolderListViewModel>(EFolderCompareList.CLEAR);
        }
        private CodeDiffReulstModel<T> GetCodeCompareModels<T>(IEnumerable<CodeInfoModel> codeInfos)
        {
            var diffFileModel = new CodeDiffReulstModel<T>();
            List<string> classFile = new List<string>();
            List<string> classFilePath = new List<string>();
            foreach (var item in codeInfos)
            {
                if (item.ComparisonResult == false)
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
            IsEnableOutputDirectoryList = true;
            _codeInfos.Add(directorySearchResult);
        }

        private async Task CompareModelCollections(IList<CodeInfoModel> inputItems, IList<CodeInfoModel> outputItems)
        {

            int i = 0, j = 0;
            while (i < inputItems.Count && j < outputItems.Count)
            {
                SWDetailedItem detailedItem = new SWDetailedItem();
                CodeInfoModel model1 = inputItems[i];
                CodeInfoModel model2 = outputItems[j];

                int comparison = string.Compare(model1.FileName, model2.FileName);
                if (comparison == 0)
                {
                    bool comparisonResult = model1.Equals(model2);

                    model1.ComparisonResult = comparisonResult;
                    model2.ComparisonResult = comparisonResult;

                    i++;
                    j++;
                    if (comparisonResult == false)
                    {                     
                        AddCodeCompreResult(_codeCompareService.GetCompareResult(model1, model2));

                        _code1.Add(model1);
                        _code2.Add(model2);

                        model1.ComparisonResult = false;
                        model2.ComparisonResult = false;

                        detailedItem.을Content = $"""
                            표 24 서버 정보융합(운용통제용) 원본(소스)파일 목록 순번 00 파일명 {model1.FileName}버전 2 크기 {model1.FileSize}체크섬 {"0x" + model1.Checksum}생성일자 {model1.CreateDate} 라인수 {model1.LineCount} 기능설명 
                            """;
                        detailedItem.으로Content = $"""
                            표 24 서버 정보융합(운용통제용) 원본(소스)파일 목록 순번 00 파일명 {model2.FileName}버전 3 크기 {model2.FileSize}체크섬 {"0x" + model2.Checksum}생성일자 {model2.CreateDate} 라인수 {model2.LineCount} 기능설명 
                            """;
                        _detailStatementItems.Add(detailedItem);
                    }
                }
                else if (comparison < 0)
                {
                    AddCodeCompreResult(_codeCompareService.GetCompareResult(model1, new CodeInfoModel()));
                    model1.ComparisonResult = false;
                    _code1.Add(model1);
                    i++;
                    detailedItem.을Content = $"""
                            표 24 서버 정보융합(운용통제용) 원본(소스)파일 목록 순번 00 파일명 {model1.FileName}버전 2 크기 {model1.FileSize}체크섬 {"0x" + model1.Checksum}생성일자 {model1.CreateDate} 라인수 {model1.LineCount} 기능설명 
                            """;
                    detailedItem.으로Content = $"""
                            -
                            """;
                    _detailStatementItems.Add(detailedItem);
                }
                else
                {                    
                    AddCodeCompreResult(_codeCompareService.GetCompareResult(new CodeInfoModel(), model2));

                    model2.ComparisonResult = false;
                    _code2.Add(model2);
                    j++;
                    detailedItem.을Content = $"""
                            -
                            """;
                    detailedItem.으로Content = $"""
                            표 24 서버 정보융합(운용통제용) 원본(소스)파일 목록 순번 00 파일명 {model2.FileName}버전 3 크기 {model2.FileSize}체크섬 {"0x" + model2.Checksum}생성일자 {model2.CreateDate} 라인수 {model2.LineCount} 기능설명 
                            """;
                    _detailStatementItems.Add(detailedItem);
                }
                
            }

            // Remaining elements in collection1 are not in collection2
            while (i < inputItems.Count)
            {
                inputItems[i].ComparisonResult = false;
                _code1.Add(inputItems[i]);
                i++;

            }

            // Remaining elements in collection2 are not in collection1
            while (j < outputItems.Count)
            {
                outputItems[j].ComparisonResult = false;
                _code2.Add(outputItems[j]);
                j++;
            }

            //foreach (var item in outputItems)
            //{
            //    foreach (var item2 in inputItems)
            //    {
            //        if (item.Equals(item2))
            //        {
            //            item.ComparisonResult = true;
            //            item2.ComparisonResult = true;
            //            break;
            //        }
            //    }
            //    if (item.ComparisonResult == false)
            //    {
            //        _code2.Add(item);
            //    }
            //}
            _code2.Distinct();
            _code1.Distinct();            
        }
        private bool AddCodeCompreResult(CustomCodeComparer customCodeComparer)
        {
            if(customCodeComparer is null)
                 return false;
            if (_codeCompareModel.CompareResults.Contains(customCodeComparer) == false)
            {
                _codeCompareModel.CompareResults.Add(customCodeComparer);
            }
            return true;
        }
    }
}
