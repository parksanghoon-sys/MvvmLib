using System.ComponentModel;

namespace CoreMvvmLib.Core.Services.RegionManager
{
    public interface IRegionManager
    {
        public void RegisterAddView(Type type);
        public void RegisterAddView<TView>() where TView : class;
        public void RegisterAddView(string regionName, Type type);
        public void RegisterAddView<TView>(string regionName) where TView : class;
        public void NavigateView(string regionName, string viewName);
        public void NavigateView(string regionName, string viewName, INotifyPropertyChanged viewModel);
        
    }
}
