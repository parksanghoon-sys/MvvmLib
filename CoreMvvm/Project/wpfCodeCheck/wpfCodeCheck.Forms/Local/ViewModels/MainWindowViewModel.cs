using CoreMvvmLib.Component.UI.Units;
using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Components;
using CoreMvvmLib.Core.Services.RegionManager;
using wpfCodeCheck.Main.UI.Views;
using CoreMvvmLib.Core.Services.DialogService;
using CoreMvvmLib.Core.Messenger;
using wpfCodeCheck.Share.Enums;
using wpfCodeCheck.ConfigurationChange.UI.Views;
using wpfCodeCheck.Shared.Local.Services;

namespace wpfCodeCheck.Forms.Local.ViewModels
{
    public class NavigationModeal
    {        
        public IconType IconType { get; set; }
        public string Name { get; set; }
        public bool IsEnable { get; set; } = true;
        
        public NavigationModeal(IconType type, string name, bool isEnable)
        {
            this.IconType = type;
            this.Name = name;
            this.IsEnable = isEnable;
        }
    }
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IDialogService _dialogService;
        [Property]
        private List<NavigationModeal> _sampleDatas = new List<NavigationModeal>()
        {
            new NavigationModeal(IconType.Home, "File CheckSum", true),
            new NavigationModeal(IconType.FileCheck, "File Export",true),
            new NavigationModeal(IconType.ViewCompact, "Project Output",false)
        };
        [Property]
        private bool _isDImming = false;


        public MainWindowViewModel(IRegionManager regionManager, IDialogService dialogService)
        {            
            _regionManager = regionManager;
            _dialogService = dialogService;            
            
            this._regionManager.NavigateView("MainContent", nameof(FolderCompareView));
            WeakReferenceMessenger.Default.Register<MainWindowViewModel, EMainViewDimming>(this, OnReceive);
        }

        private void OnReceive(MainWindowViewModel model, EMainViewDimming isDImming)
        {
            if (isDImming == EMainViewDimming.NONE)
                IsDImming = false;
            else
                IsDImming = true;
            
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
                        this._regionManager.NavigateView("MainContent", nameof(ComparisonResultsView));                     
                    }
                    break;
                case IconType.ViewCompact:
                    {
                        //_dialogService.Close(nameof(LoadingDialogView));
                        //this._regionManager.NavigateView("MainContent", nameof(Test3View));

                    }
                    break;
            }
            
        }

    }
}
