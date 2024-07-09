using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Components;
using CoreMvvmLib.Core.Messenger;
using CoreMvvmLib.WPF.Components;
using System.Collections.Generic;
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
        private void Compare()
        {
            var inputItems = _codeInfos.First();
            var outputItems = _codeInfos.Last();

            CompareModelCollections(inputItems, outputItems);
            IList<CodeCompareModel> inputDiffCodeList = new List<CodeCompareModel>();
            IList<CodeCompareModel> outputDiffCodeList = new List<CodeCompareModel>();

            _baseService.SetCodeInfos(GetCodeCompareModels(inputItems), GetCodeCompareModels(outputItems));
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
        private IList<CodeCompareModel> GetCodeCompareModels(IEnumerable<CodeInfo> codeInfos)
        {
            IList<CodeCompareModel> diffCodeList = new List<CodeCompareModel>();
            foreach (var item in codeInfos)
            {
                if (item.ComparisonResult == false)
                {
                    diffCodeList.Add(new CodeCompareModel()
                    {
                        ClassNmae = item.FileName,
                        FilePath = item.FilePath
                    });
                }                
            }
            return diffCodeList;
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

        private void CompareModelCollections(IList<CodeInfo> collection1, IList<CodeInfo> collection2)
        {
            int i = 0, j = 0;
            while (i < collection1.Count && j < collection2.Count)
            {
                CodeInfo model1 = collection1[i];
                CodeInfo model2 = collection2[j];

                int comparison = string.Compare(model1.FileName, model2.FileName);
                if (comparison == 0)
                {
                    bool comparisonResult = model1.Equals(model2);

                    model1.ComparisonResult = comparisonResult;
                    model2.ComparisonResult = comparisonResult;

                    i++;
                    j++;
                    if(comparisonResult == false)
                    {
                        _code1.Add(model1);
                        _code2.Add(model2);
                    }
                }
                else if (comparison < 0)
                {
                    model1.ComparisonResult = false;
                    _code1.Add(model1);
                    i++;
                }
                else
                {
                    model2.ComparisonResult = false;
                    _code2.Add(model2);
                    j++;
                }
            }

            // Remaining elements in collection1 are not in collection2
            while (i < collection1.Count)
            {
                collection1[i].ComparisonResult = false;
                _code1.Add(collection1[i]);
                i++;
                
            }

            // Remaining elements in collection2 are not in collection1
            while (j < collection2.Count)
            {
                collection2[j].ComparisonResult = false;
                _code2.Add(collection2[j]);
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
        }
    }
}
