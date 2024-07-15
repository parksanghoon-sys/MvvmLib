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
            new NavigationModeal(IconType.Home, "Compare Directory", true),
            new NavigationModeal(IconType.FileCheck, "File CheckSum",true),
            new NavigationModeal(IconType.Export, "Excel Output",true),
            new NavigationModeal(IconType.FileWord, "SDD Output",true)
        };
        [Property]
        private bool _isDImming = false;
        [Property]
        private int _selectedIndex;

        public MainWindowViewModel(IRegionManager regionManager, IDialogService dialogService)
        {
            _regionManager = regionManager;
            _dialogService = dialogService;            

            this._regionManager.NavigateView("MainContent", nameof(DirectoryCompareView));
            WeakReferenceMessenger.Default.Register<MainWindowViewModel, EMainViewDimming>(this, OnReceiveDimming);
            WeakReferenceMessenger.Default.Register<MainWindowViewModel, EMainViewType>(this, OnReceiveMainContentViewOnChange);            
        }
        [RelayCommand]
        private void TabItemSelected(NavigationModeal model)
        {
            switch (model.IconType)
            {
                case IconType.Home:
                    {
                        this._regionManager.NavigateView("MainContent", nameof(DirectoryCompareView));
                        SelectedIndex = 0;
                    }
                    break;
                case IconType.FileCheck:
                    {
                        this._regionManager.NavigateView("MainContent", nameof(FolderCompareView));
                        SelectedIndex = 1;
                    }
                    break;
                case IconType.Export:
                    {                        
                        this._regionManager.NavigateView("MainContent", nameof(ComparisonResultsView));
                        SelectedIndex = 2;
                    }
                    break;
                case IconType.FileWord:
                    {
                        this._regionManager.NavigateView("MainContent", nameof(ComparisonResultsView));
                        SelectedIndex = 3;
                    }
                    break;
            }
        }
        private void OnReceiveMainContentViewOnChange(MainWindowViewModel model, EMainViewType type)
        {
            SelectedIndex = type switch
            {
                EMainViewType.HOME => 0,
                EMainViewType.FILE_CHECKSUM => 1,
                EMainViewType.EXPORT_EXCEL => 2,
                _ =>  3
            };
        }

        private void OnReceiveDimming(MainWindowViewModel model, EMainViewDimming isDImming)
        {
            if (isDImming == EMainViewDimming.NONE)
                IsDImming = false;
            else
                IsDImming = true;

        }

      

    }
}
