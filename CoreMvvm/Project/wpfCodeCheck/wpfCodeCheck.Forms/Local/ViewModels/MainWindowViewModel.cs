using CoreMvvmLib.Component.UI.Units;
using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Commands;
using CoreMvvmLib.Core.Components;
using CoreMvvmLib.Core.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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
        [Property]
        private List<NavigationModeal> _sampleDatas;        
        [Property]
        private int _myVar;
        [Property]
        private object _mainContent;        
        
        public MainWindowViewModel()
        {            
            _sampleDatas = new List<NavigationModeal>();
            this.SampleDatas = new List<NavigationModeal>();
            this.SampleDatas.Add(new NavigationModeal(IconType.Home, "Home"));
            this.SampleDatas.Add(new NavigationModeal(IconType.FileCheck, "FIle CheckSum"));
            this.SampleDatas.Add(new NavigationModeal(IconType.ViewCompact, "File Compare"));            
        }
        [RelayCommand]
        private void TabItemSelected(NavigationModeal model)
        {

            switch (model.IconType)
            {
                case IconType.Home:
                    {                        
                        MainContent = ServiceContainer.Instance().GetService<TestViewModel>();
                    }
                    break;
                case IconType.FileCheck:
                    {                        
                    }
                    break;
                case IconType.ViewCompact:
                    {
                     
                    }
                    break;
            }
        }

    }
}
