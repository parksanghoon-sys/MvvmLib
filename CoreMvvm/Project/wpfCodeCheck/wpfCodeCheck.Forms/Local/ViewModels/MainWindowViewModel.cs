using CoreMvvmLib.Component.UI.Units;
using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Components;
using CoreMvvmLib.Core.Services.RegionManager;
using wpfCodeCheck.Main.UI.Views;
using CoreMvvmLib.Core.Services.DialogService;
using CoreMvvmLib.Core.Messenger;
using wpfCodeCheck.ProjectChangeTracker.UI.Views;
using wpfCodeCheck.Forms.Local.Models;
using wpfCodeCheck.Domain.Services;
using wpfCodeCheck.Domain.Enums;

namespace wpfCodeCheck.Forms.Local.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IDialogService _dialogService;
        private readonly ISettingService _settingService;
        [Property]
        private List<NavigationModeal> _sliderMenuList = new List<NavigationModeal>()
        {
            new NavigationModeal(IconType.Home, "Compare Directory", true),
            new NavigationModeal(IconType.FileCheck, "File CheckSum",false),
            new NavigationModeal(IconType.Export, "Excel Output",false),
            new NavigationModeal(IconType.FileWord, "SDD Output",false)
        };
        [Property]
        private bool _isDImming = false;
        [Property]
        private int _selectedIndex;
        [Property]
        public double _windowLeft = 0;
        [Property]
        public double _windowTop = 0;
        [Property]  
        public double _windowWidth = 0;
        [Property]
        public double _windowHeight = 0;

        public MainWindowViewModel(IRegionManager regionManager, IDialogService dialogService, ISettingService settingService)
        {
            _regionManager = regionManager;
            _dialogService = dialogService;
            _settingService = settingService;            

            WindowLeft = _settingService.WindowSetting!.XPos ?? 500;
            WindowTop = _settingService.WindowSetting!.YPos ??500;
            WindowWidth = 1200;
            WindowHeight = _settingService.WindowSetting!.Height ?? 600;

            this._regionManager.NavigateView(ERegionManager.MAINCONTENT.ToString(), nameof(DirectoryCompareView));
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
                        this._regionManager.NavigateView(ERegionManager.MAINCONTENT.ToString(), nameof(DirectoryCompareView));
                        SelectedIndex = 0;
                    }
                    break;
                case IconType.FileCheck:
                    {
                        this._regionManager.NavigateView(ERegionManager.MAINCONTENT.ToString(), nameof(FolderCompareView));                        
                        SelectedIndex = 1;
                    }
                    break;
                case IconType.Export:
                    {                        
                        this._regionManager.NavigateView(ERegionManager.MAINCONTENT.ToString(), nameof(ComparisonResultsView));
                        SelectedIndex = 2;
                    }
                    break;
                case IconType.FileWord:
                    {
                        this._regionManager.NavigateView(ERegionManager.MAINCONTENT.ToString(), nameof(ComparisonResultsView));
                        SelectedIndex = 3;
                    }
                    break;
            }
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
                _ =>1
            };
            SliderMenuList[(int)type].IsEnable = true;
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
