using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Components;
using CoreMvvmLib.Core.Messenger;

namespace wpfCodeCheck.Component.Local.ViewModels
{
    public partial class CircularProgressBarViewModel : ViewModelBase
    {
        [Property]
        private double _progress;
        public CircularProgressBarViewModel()
        {
            WeakReferenceMessenger.Default.Register<CircularProgressBarViewModel, double>(this, OnReceiveProgress);
        }

        private void OnReceiveProgress(CircularProgressBarViewModel model, double arg2)
        {
            Progress = arg2; 
        }
    }
}
