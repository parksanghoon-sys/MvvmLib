using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Commands;
using CoreMvvmLib.Core.Components;
using CoreMvvmLib.Core.Services.DialogService;
using System.Windows.Input;

namespace WpfTest1.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;
        public MainWindowViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        [Property]
        private string testText;
        public ICommand TestCommand => new RelayCommand(new Action(() =>
        {
            _dialogService.Show(this, "TestDialogView", 300, 200);
        }));
        public ICommand TestCommandAsync => new RelayCommandAsync(async () =>
        {
        });
        public ICommand Loaded1Command => new RelayCommand(new Action(() =>
        {
            System.Diagnostics.Debug.WriteLine("View Loaded!!");
        }));
        public MainWindowViewModel()
        {

        }

    }
}
