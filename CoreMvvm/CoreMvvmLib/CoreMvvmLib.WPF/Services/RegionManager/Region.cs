using CoreMvvmLib.Core.Services.RegionManager;
using System.ComponentModel;
using System.Windows.Controls;

namespace CoreMvvmLib.WPF.Services.RegionManager
{
    internal class Region : IRegion
    {
        private List<WeakReference> _viewReferenceList = new List<WeakReference>();
        public Region(string sourceViewName)
        {
            SourceViewName = sourceViewName;
        }
        public List<WeakReference> Sources
        {
            get
            {
                var deadControl = _viewReferenceList.Where(data => data.IsAlive == false).ToList();
                foreach(var control in  deadControl)
                {
                    _viewReferenceList.Remove(control);
                }
                return  _viewReferenceList;
            }
        }
        public string SourceViewName { get; set; } = "";


        public event Action<object, object>? OnContentUpdate;

        public void NavigateView()
        {
            try
            {
                if(OnContentUpdate != null)
                {                    
                    var source = Sources;
                    foreach(var control in source)
                    {
                        var view = ViewTypeStore.CreateView(SourceViewName);

                        var contentControl = control.Target as ContentControl;
                        if (contentControl == null) continue;
                        this.OnContentUpdate(contentControl, view);
                    }
                }
            }
            catch (Exception ex)
            {

                throw new InvalidOperationException(ex.Message);
            }
        }

        public void NavigateView(INotifyPropertyChanged viewModel)
        {
            try
            {
                if (this.OnContentUpdate != null)
                {
                    var view = ViewTypeStore.CreateView(this.SourceViewName);
                    var sources = this.Sources;
                    foreach (var source in sources)
                    {
                        var contentControl = source.Target as ContentControl;
                        if (contentControl == null) continue;
                        contentControl.DataContext = viewModel;
                        this.OnContentUpdate(contentControl, view);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);                
            }
        }
    }
}
