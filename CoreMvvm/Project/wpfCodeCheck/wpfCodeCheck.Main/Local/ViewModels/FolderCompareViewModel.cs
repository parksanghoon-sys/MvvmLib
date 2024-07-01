using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Components;
using CoreMvvmLib.Core.Messenger;
using CoreMvvmLib.WPF.Components;
using wpfCodeCheck.Main.Local.Models;
using static Microsoft.CodeAnalysis.AssemblyIdentityComparer;

namespace wpfCodeCheck.Main.Local.ViewModels
{
    public partial class FolderCompareViewModel : ViewModelBase
    {
        private List<CustomObservableCollection<CodeInfo>> _codeInfos = new List<CustomObservableCollection<CodeInfo>>(2);
        private List<CodeInfo> _code1 = new List<CodeInfo>();
        private List<CodeInfo> _code2 = new List<CodeInfo>();
        public FolderCompareViewModel()
        {
            WeakReferenceMessenger.Default.Register<FolderCompareViewModel, CustomObservableCollection<CodeInfo>>(this, OnReceiveCodeInfos);
        }
        [RelayCommand]
        private void Compare()
        {
            var item1 = _codeInfos.First();
            var item2 = _codeInfos.Last();   

            CompareModelCollections(item1, item2);            
        }
        [RelayCommand]
        private void Export()
        {

        }
        private void OnReceiveCodeInfos(FolderCompareViewModel model, CustomObservableCollection<CodeInfo> list)
        {
            if(_codeInfos.Count > 2)
            {
                _codeInfos.Clear();
            }
            _codeInfos.Add(list);
        }

        private void CompareModelCollections(IList<CodeInfo> collection1, IList<CodeInfo> collection2)
        {
            //int i = 0, j = 0;
            //while (i < collection1.Count && j < collection2.Count)
            //{
            //    CodeInfo model1 = collection1[i];
            //    CodeInfo model2 = collection2[j];          

            //    int comparison = string.Compare(model1.FileName, model2.FileName);
            //    if (comparison == 0)
            //    {
            //        bool comparisonResult = model1.Equals(model2);
            //        if (comparisonResult == true)
            //        {
            //            model1.ComparisonResult = comparisonResult;
            //            model2.ComparisonResult = comparisonResult;
            //        }

            //        i++;
            //        j++;
            //    }
            //    else if (comparison < 0)
            //    {
            //        model1.ComparisonResult = false;
            //        i++;
            //    }
            //    else
            //    {
            //        model2.ComparisonResult = false;
            //        j++;
            //    }
            //}

            //// Remaining elements in collection1 are not in collection2
            //while (i < collection1.Count)
            //{
            //    collection1[i].ComparisonResult = false;
            //    i++;
            //}

            //// Remaining elements in collection2 are not in collection1
            //while (j < collection2.Count)
            //{
            //    collection2[j].ComparisonResult = false;
            //    j++;
            //}

            foreach (var item in collection1)
            {
                foreach(var item2 in collection2)
                {
                    if(item.Equals(item2))
                    {
                        item.ComparisonResult = true;
                        item2.ComparisonResult = true;
                    }
                }
                if(item.ComparisonResult == false)
                {
                    _code1.Add(item);
                }
            }
        }
    }
}
