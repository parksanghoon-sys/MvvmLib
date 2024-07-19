using CompareEngine;
using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Components;
using CoreMvvmLib.Core.Messenger;
using System.Collections;
using System.IO;
using System.Windows;
using wpfCodeCheck.Main.Local.Exceptions;
using wpfCodeCheck.Main.Local.Helpers.CsvHelper;
using wpfCodeCheck.Main.Local.Models;
using wpfCodeCheck.Domain.Enums;
using wpfCodeCheck.Domain.Datas;
using wpfCodeCheck.Domain.Services;

namespace wpfCodeCheck.Main.Local.ViewModels
{
    public partial class FolderCompareViewModel : ViewModelBase
    {
        private List<DirectorySearchResult> _codeInfos = new List<DirectorySearchResult>(2);
        private List<CodeInfoModel> _code1 = new List<CodeInfoModel>();
        private List<CodeInfoModel> _code2 = new List<CodeInfoModel>();
        private CodeCompareResultModel _codeCompareModel = new CodeCompareResultModel();

        private readonly ICsvHelper _csvHelper;
        private readonly IBaseService _baseService;
        private readonly ISettingService _settingService;

        public FolderCompareViewModel(ICsvHelper csvHelper, IBaseService baseService, ISettingService settingService)
        {
            _csvHelper = csvHelper;
            _baseService = baseService;
            _settingService = settingService;
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
        private string _inputDirectoryPath;
        [Property]
        private string _outputDirectoryPath;
        [AsyncRelayCommand]
        private async Task Compare()
        {
            if (_codeInfos.Count != 2)
            {
                throw new InsufficientDataException($"파일 데이터가 부족 합니다.");
            }
            var inputItems = _codeInfos.Where(p => p.type == EFolderListType.INPUT).FirstOrDefault();;
            var outputItems = _codeInfos.Where(p => p.type == EFolderListType.OUTPUT).FirstOrDefault();

            await CompareModelCollections(inputItems!.fileDatas, outputItems!.fileDatas);
            
            _baseService.SetDirectoryCompareReuslt(_codeCompareModel);
            WeakReferenceMessenger.Default.Send<EMainViewType>(EMainViewType.EXPORT_EXCEL);
        }
        [RelayCommand]
        private void Export()
        {
            _csvHelper.CreateCSVFile<CodeInfoModel>(_code2, "codeinfo2");
            MessageBox.Show("완료");
        }
        [RelayCommand]
        private void Clear()
        {
            WeakReferenceMessenger.Default.Send<EFolderCompareList, FolderListViewModel>(EFolderCompareList.CLEAR);
        }
        private CodeCompareResultModel GetCodeCompareModels(IEnumerable<CodeInfoModel> codeInfos)
        {
            var diffFileModel = new CodeCompareResultModel();
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
            await Task.Run(() =>
            {
                int i = 0, j = 0;
                while (i < inputItems.Count && j < outputItems.Count)
                {
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
                            var compareResult = GetCompareResult(model1.FilePath, model2.FilePath, model1.FileName);
                            if (_codeCompareModel.CompareResults.ContainsKey(model1.ProjectName) == true)
                            {
                                _codeCompareModel.CompareResults[model1.ProjectName].Add(compareResult);
                            }
                            else
                            {
                                _codeCompareModel.CompareResults.Add(model1.ProjectName,new List<CompareResult> {compareResult});
                            }
                            _code1.Add(model1);
                            _code2.Add(model2);

                            model1.ComparisonResult = false;
                            model2.ComparisonResult = false;
                        }
                    }
                    else if (comparison < 0)
                    {
                        var compareResult = GetCompareResult(model1.FilePath, "", model1.FileName);

                        if (_codeCompareModel.CompareResults.ContainsKey(model1.ProjectName) == true)
                        {
                            _codeCompareModel.CompareResults[model1.ProjectName].Add(compareResult);
                        }
                        else
                        {
                            _codeCompareModel.CompareResults.Add(model1.ProjectName, new List<CompareResult> { compareResult });
                        }
                        model1.ComparisonResult = false;
                        _code1.Add(model1);
                        i++;
                    }
                    else
                    {
                        var compareResult = GetCompareResult("", model2.FilePath, model2.FileName);

                        if (_codeCompareModel.CompareResults.ContainsKey(model1.ProjectName) == true)
                        {
                            _codeCompareModel.CompareResults[model1.ProjectName].Add(compareResult);
                        }
                        else
                        {
                            _codeCompareModel.CompareResults.Add(model1.ProjectName, new List<CompareResult> { compareResult });
                        }                        

                        model2.ComparisonResult = false;
                        _code2.Add(model2);
                        j++;
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
            });
                
        }
        private CompareResult GetCompareResult(string sourcePath, string destinationPath, string fileName)
        {
            CompareText sourceDiffList;
            CompareText destinationDiffList;

            if (File.Exists(sourcePath))
                sourceDiffList = new CompareText(sourcePath);
            else
                sourceDiffList = new CompareText();

            if (File.Exists(destinationPath))
                destinationDiffList = new CompareText(destinationPath);
            else
                destinationDiffList = new CompareText();
             
            CompareEngine.CompareEngine compareEngine = new CompareEngine.CompareEngine();
            compareEngine.StartDiff(sourceDiffList, destinationDiffList);

            ArrayList resultLines = compareEngine.DiffResult();
            CompareResult compareResult = new CompareResult();
            compareResult.InputCompareText = sourceDiffList;
            compareResult.OutputCompareText = destinationDiffList;
            compareResult.CompareResultSpans = GetArrayListToList<CompareResultSpan>(resultLines);
            compareResult.FileName = fileName;
            
            return compareResult;
        }

        private IList<T> GetArrayListToList<T>(ArrayList list)
        {            
            List<T> result = new List<T>();

            foreach (var item in list)
            {
                if (item is T)
                {
                    result.Add((T)item);
                }
            }

            return result;
        }
    }
}
