using CoreMvvmLib.Component.UI.Units;
using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Commands;
using CoreMvvmLib.Core.Components;
using CoreMvvmLib.Core.Services.RegionManager;
using wpfCodeCheck.Forms.Themes.Views;
using wpfCodeCheck.Sub.UI.Views;
using wpfCodeCheck.Main.UI.Views;
using CoreMvvmLib.Core.Services.DialogService;

namespace wpfCodeCheck.Forms.Local.ViewModels
{
    public class NavigationModeal
    {        
        public IconType IconType { get; set; }
        public string Name { get; set; }

        public NavigationModeal(IconType type, string name)
        {
            this.IconType = type;
            this.Name = name;
        }
    }
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IDialogService _dialogService;
        [Property]
        private List<NavigationModeal> _sampleDatas;
        [Property]
        private bool _isDImming = false;


        public MainWindowViewModel(IRegionManager regionManager, IDialogService dialogService)
        {            
            _regionManager = regionManager;
            _dialogService = dialogService;
            _sampleDatas = new List<NavigationModeal>()
            {
                   new NavigationModeal(IconType.Home, "Home"),
                   new NavigationModeal(IconType.FileCheck, "FIle CheckSum"),
                   new NavigationModeal(IconType.ViewCompact, "Project Output")
            };
            
            this._regionManager.NavigateView("MainContent", nameof(FolderCompareView));            
        }
        [RelayCommand]
        private void TabItemSelected(NavigationModeal model)
        {

            switch (model.IconType)
            {
                case IconType.Home:
                    {
                        this._regionManager.NavigateView("MainContent", nameof(FolderCompareView));
                    }
                    break;
                case IconType.FileCheck:
                    {
                        _dialogService.Show(this, nameof(LoadingDialogView),300, 300);
                        this._regionManager.NavigateView("MainContent", nameof(Test2View));     
                    }
                    break;
                case IconType.ViewCompact:
                    {
                        _dialogService.Close(nameof(LoadingDialogView));
                        this._regionManager.NavigateView("MainContent", nameof(Test3View));

                    }
                    break;
            }
            
        }

    }
}
