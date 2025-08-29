using CoreMvvmLib.Component.UI.Units;
using DomainIconType = wpfCodeCheck.Domain.Enums.IconType;
using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Components;
using CoreMvvmLib.Core.Services.RegionManager;
using wpfCodeCheck.Main.UI.Views;
using CoreMvvmLib.Core.Services.DialogService;
using CoreMvvmLib.Core.Messenger;
using wpfCodeCheck.ProjectChangeTracker.UI.Views;
using wpfCodeCheck.Domain.Models;
using wpfCodeCheck.Domain.Enums;
using wpfCodeCheck.SDDExport.UI.Views;
using wpfCodeCheck.Domain.Services.Interfaces;

namespace wpfCodeCheck.Forms.Local.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IDialogService _dialogService;
        private readonly ISettingService _settingService;
        [Property]
        private List<NavigationModel> _sliderMenuList = new List<NavigationModel>()
        {
            new NavigationModel(DomainIconType.Home, "Compare Directory", true),
            new NavigationModel(DomainIconType.FileCheck, "File CheckSum",false),
            new NavigationModel(DomainIconType.Export, "Excel Output",false),
            new NavigationModel(DomainIconType.FileWord, "SDD Output",false)
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
        private void TabItemSelected(NavigationModel model)
        {
            switch (model.IconType)
            {
                case DomainIconType.Home:
                    {
                        this._regionManager.NavigateView(ERegionManager.MAINCONTENT.ToString(), nameof(DirectoryCompareView));
                        SelectedIndex = 0;
                    }
                    break;
                case DomainIconType.FileCheck:
                    {
                        this._regionManager.NavigateView(ERegionManager.MAINCONTENT.ToString(), nameof(FolderCompareView));                        
                        SelectedIndex = 1;
                    }
                    break;
                case DomainIconType.Export:
                    {                        
                        this._regionManager.NavigateView(ERegionManager.MAINCONTENT.ToString(), nameof(ComparisonResultsView));
                        SelectedIndex = 2;
                    }
                    break;
                case DomainIconType.FileWord:
                    {
                        this._regionManager.NavigateView(ERegionManager.MAINCONTENT.ToString(), nameof(SddExportView));
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
            if(type == EMainViewType.EXPORT_EXCEL)
            {
                SliderMenuList[(int)type].IsEnable = true;
                SliderMenuList[(int)type + 1].IsEnable = true;
            }
            
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
