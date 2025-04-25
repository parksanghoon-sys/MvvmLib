using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Commands;
using CoreMvvmLib.Core.Components;
using CoreMvvmLib.Core.Services.RegionManager;
using System.Windows.Input;

namespace RegionTest.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        #region Private Property
        private readonly IRegionManager regionManager;
        public ICommand TestCommand { get; set; }
        #endregion

        #region Constructor
        public MainWindowViewModel(IRegionManager regionManager)
        {

            this.regionManager = regionManager;
            TestCommand = new RelayCommand(Test);
        }
        #endregion

        #region Command
        [RelayCommand]
        public void Test()
        {
            this.regionManager.NavigateView("MainContent", "AView");
        }
        #endregion
    }
}
