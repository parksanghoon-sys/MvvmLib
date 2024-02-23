using System.ComponentModel;

namespace CoreMvvmLib.Core.Services.RegionManager
{
    public interface IRegion
    {
        public string SourceViewName { get; set; }
        public List<WeakReference> Sources { get; }

        public void NavigateView();
        public void NavigateView(INotifyPropertyChanged viewModel);

        public event Action<object, object> OnContentUpdate;
    }
}
