using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Commands;
using CoreMvvmLib.Core.Components;
using System.Windows.Input;

namespace WpfTest
{
    public partial class MainViewModel : ViewModelBase
    {
        

        [Property]
        private string testText;
        public ICommand TestCommand => new RelayCommand(new Action(() =>
        {
            
        }));
        public ICommand TestCommandAsync => new RelayCommandAsync(async () =>
        {            
        });
        public ICommand Loaded1Command => new RelayCommand(new Action(() =>
        {
            System.Diagnostics.Debug.WriteLine("View Loaded!!");
        }));
        public MainViewModel() 
        {
            
        }

    }
}
