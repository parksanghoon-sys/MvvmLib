using CoreMvvmLib.Component.UI.Units;
using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Commands;
using CoreMvvmLib.Core.Components;
using CoreMvvmLib.Core.IOC;
using CoreMvvmLib.Core.Services.RegionManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using wpfCodeCheck.Forms.Themes.Views;
using wpfCodeCheck.Forms.UI.Views;

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
        [Property]
        private List<NavigationModeal> _sampleDatas;                        
        
        public MainWindowViewModel(IRegionManager regionManager)
        {            
            _sampleDatas = new List<NavigationModeal>();
            this.SampleDatas = new List<NavigationModeal>();
            this.SampleDatas.Add(new NavigationModeal(IconType.Home, "Home"));
            this.SampleDatas.Add(new NavigationModeal(IconType.FileCheck, "FIle CheckSum"));
            this.SampleDatas.Add(new NavigationModeal(IconType.ViewCompact, "File Compare"));
            _regionManager = regionManager;
            this._regionManager.NavigateView("MainContent", nameof(TestView));
        }
        [RelayCommand]
        private void TabItemSelected(NavigationModeal model)
        {

            switch (model.IconType)
            {
                case IconType.Home:
                    {
                        this._regionManager.NavigateView("MainContent", nameof(TestView));
                    }
                    break;
                case IconType.FileCheck:
                    {
                        this._regionManager.NavigateView("MainContent", nameof(Test2View));
                    }
                    break;
                case IconType.ViewCompact:
                    {
                        this._regionManager.NavigateView("MainContent", nameof(Test3View));

                    }
                    break;
            }
            
        }

    }
}
