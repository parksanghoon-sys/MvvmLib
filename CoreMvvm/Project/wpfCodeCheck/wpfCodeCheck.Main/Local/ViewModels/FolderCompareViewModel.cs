using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Components;
using CoreMvvmLib.Core.Messenger;
using CoreMvvmLib.WPF.Components;
using wpfCodeCheck.Main.Local.Models;

namespace wpfCodeCheck.Main.Local.ViewModels
{
    public partial class FolderCompareViewModel : ViewModelBase
    {
        private Queue<CustomObservableCollection<CodeInfo>> _codeInfos = new Queue<CustomObservableCollection<CodeInfo>>(2);
        public FolderCompareViewModel()
        {
            WeakReferenceMessenger.Default.Register<FolderCompareViewModel, CustomObservableCollection<CodeInfo>>(this, OnReceiveCodeInfos);
        }
        [RelayCommand]
        private void Compare()
        {
            var item1 = _codeInfos.Dequeue();
            var item2 = _codeInfos.Dequeue();   

            CompareModelCollections(item1, item2);
        }
        private void OnReceiveCodeInfos(FolderCompareViewModel model, CustomObservableCollection<CodeInfo> list)
        {
            if(_codeInfos.Count > 2)
            {
                _codeInfos.Dequeue();
            }
            _codeInfos.Enqueue(list);
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
                    if (comparisonResult == true)
                    {
                        model1.ComparisonResult = !comparisonResult;
                        model2.ComparisonResult = !comparisonResult;
                    }

                    i++;
                    j++;
                }
                else if (comparison < 0)
                {
                    model1.ComparisonResult = true;
                    i++;
                }
                else
                {
                    model2.ComparisonResult = true;
                    j++;
                }
            }

            // Remaining elements in collection1 are not in collection2
            while (i < collection1.Count)
            {
                collection1[i].ComparisonResult = true;
                i++;
            }

            // Remaining elements in collection2 are not in collection1
            while (j < collection2.Count)
            {
                collection2[j].ComparisonResult = true;
                j++;
            }
        }
    }
}
