using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Commands;
using CoreMvvmLib.Core.ViewModelBase;
using System.Windows.Input;

namespace WpfTest
{
    public class MainViewModel : ViewModelBase
    {
        

        [Property]
        private string _testText;
        public ICommand TestCommand => new RelayCommand(new Action(() =>
        {
            TestText = "TestTextCommand";
        }));
        public ICommand TestCommandAsync => new RelayCommandAsync(async () =>
        {
            TestText = "TestTextAsyncCommand";
        });
        public MainViewModel() { }

    }
}
