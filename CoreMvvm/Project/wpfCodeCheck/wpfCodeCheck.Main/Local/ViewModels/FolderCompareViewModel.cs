using CompareEngine;
using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Components;
using CoreMvvmLib.Core.Messenger;
using CoreMvvmLib.WPF.Components;
using System.Collections;
using System.IO;
using System.Windows;
using wpfCodeCheck.Main.Local.Helpers.CsvHelper;
using wpfCodeCheck.Main.Local.Models;
using wpfCodeCheck.Share.Enums;
using wpfCodeCheck.Shared.Local.Models;
using wpfCodeCheck.Shared.Local.Services;

namespace wpfCodeCheck.Main.Local.ViewModels
{
    public partial class FolderCompareViewModel : ViewModelBase
    {
        private List<CustomObservableCollection<CodeInfo>> _codeInfos = new List<CustomObservableCollection<CodeInfo>>(2);
        private List<CodeInfo> _code1 = new List<CodeInfo>();
        private List<CodeInfo> _code2 = new List<CodeInfo>();
        private CodeCompareModel _codeCompareModel = new CodeCompareModel();
        private readonly ICsvHelper _csvHelper;
        private readonly IBaseService _baseService;

        public FolderCompareViewModel(ICsvHelper csvHelper, IBaseService baseService)
        {
            _csvHelper = csvHelper;
            _baseService = baseService;
            IsEnableInputDirectoryList = true;
            IsEnableOutputDirectoryList = false;

            WeakReferenceMessenger.Default.Register<FolderCompareViewModel, CustomObservableCollection<CodeInfo>>(this, OnReceiveCodeInfos);
        }
        [Property]
        private bool _isEnableInputDirectoryList;
        [Property]
        private bool _isEnableOutputDirectoryList;
        [RelayCommand]
        private async void Compare()
        {
            var inputItems = _codeInfos.First();
            var outputItems = _codeInfos.Last();

            await CompareModelCollections(inputItems, outputItems);
            _baseService.SetDirectoryCompareReuslt(_codeCompareModel);
        }
        [RelayCommand]
        private void Export()
        {
            _csvHelper.CreateCSVFile<CodeInfo>(_code2, "codeinfo2");
            MessageBox.Show("완료");
        }
        [RelayCommand]
        private void Clear()
        {
            WeakReferenceMessenger.Default.Send<EFolderCompareList, FolderListViewModel>(EFolderCompareList.CLEAR);
        }
        private CodeCompareModel GetCodeCompareModels(IEnumerable<CodeInfo> codeInfos)
        {
            var diffFileModel = new CodeCompareModel();
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
        private void OnReceiveCodeInfos(FolderCompareViewModel model, CustomObservableCollection<CodeInfo> list)
        {
            if (_codeInfos.Count >= 2)
            {
                _codeInfos.Clear();
            }
            IsEnableOutputDirectoryList = true;
            _codeInfos.Add(list);
        }

        private async Task CompareModelCollections(IList<CodeInfo> inputItems, IList<CodeInfo> outputItems)
        {
            await Task.Run(() =>
            {
                int i = 0, j = 0;
                while (i < inputItems.Count && j < outputItems.Count)
                {
                    CodeInfo model1 = inputItems[i];
                    CodeInfo model2 = outputItems[j];

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
                            _codeCompareModel.CompareResults.Add(GetCompareResult(model1.FilePath, model2.FilePath, model1.FileName ));  
                            _code1.Add(model1);
                            _code2.Add(model2);

                            model1.ComparisonResult = false;
                            model2.ComparisonResult = false;
                        }
                    }
                    else if (comparison < 0)
                    {                                                
                        _codeCompareModel.CompareResults.Add(GetCompareResult(model1.FilePath, "", model1.FileName));

                        model1.ComparisonResult = false;
                        _code1.Add(model1);
                        i++;
                    }
                    else
                    {                                                
                        _codeCompareModel.CompareResults.Add(GetCompareResult("", model2.FilePath, model1.FileName));

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

                //foreach (var item in collection1)
                //{
                //    foreach(var item2 in collection2)
                //    {
                //        if(item.Equals(item2))
                //        {
                //            item.ComparisonResult = true;
                //            item2.ComparisonResult = true;
                //        }
                //    }
                //    if(item.ComparisonResult == false)
                //    {
                //        _code1.Add(item);
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
