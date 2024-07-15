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
using wpfCodeCheck.Forms.Local.Models;

namespace wpfCodeCheck.Forms.Local.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IDialogService _dialogService;
        private readonly ISettingService _settingService;
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
        [Property]
        public double _windowLeft = 500;
        [Property]
        public double _windowTop = 500;
        [Property]  
        public double _windowWidth = 800;
        [Property]
        public double _windowHeight = 650;

        public MainWindowViewModel(IRegionManager regionManager, IDialogService dialogService, ISettingService settingService)
        {
            _regionManager = regionManager;
            _dialogService = dialogService;
            _settingService = settingService;
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
        [RelayCommand]
        private void Loaded()
        {
            int a = 0;
            a++;
        }
        [RelayCommand]
        private void Cloasing(object? param)
        {
            // 0 - Window left
            // 1 - Window top
            // 2 - Window width
            // 3 - Window height
            object[] windowInfo = (object[])param!;

            _settingService.WindowSetting!.XPos = (double)windowInfo[0];
            _settingService.WindowSetting!.YPos = (double)windowInfo[1];
            _settingService.WindowSetting!.Width = (double)windowInfo[2];
            _settingService.WindowSetting!.Height = (double)windowInfo[3];
            _settingService.SaveSetting();
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
