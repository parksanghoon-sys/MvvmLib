using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Commands;
using CoreMvvmLib.Core.Components;
using System.Windows.Input;

namespace WpfTest
{
    public class MainViewModel : ViewModelBase
    {
        

        [Property]
        private string _testText;
        public ICommand TestCommand => new RelayCommand(new Action(() =>
        {
            
        }));
        public ICommand TestCommandAsync => new RelayCommandAsync(async () =>
        {            
        });
        public MainViewModel() { }

    }
}
