using CoreMvvmLib.Core.IOC;

namespace CoreMvvmLib.Core.Services.DialogService
{
    public interface IDialogService
    {
        #region Function
        public void SetServiceProvider(IServiceContainer serviceProvider);

        public void RegisterDialog(Type type);
        public void RegisterDialog(Type type, bool isSingle);
        public void RegisterDialog<TWindow>() where TWindow : class;
        public void RegisterSingleDialog<TWindow>(bool isSingle = false) where TWindow : class;


        public bool Close(System.ComponentModel.INotifyPropertyChanged viewModel);
        public bool Close(Type dialogType);
        public bool Activate(System.ComponentModel.INotifyPropertyChanged viewModel);
        public bool Activate(Type dialogType);

        public bool ShowDialog(System.ComponentModel.INotifyPropertyChanged ownerViewModel, Type windowType, string title, int width, int height);
        public bool ShowDialog(System.ComponentModel.INotifyPropertyChanged ownerViewModel, IModalDialogViewModel viewModel, string title, int width, int height);
        public void Show(System.ComponentModel.INotifyPropertyChanged ownerViewModel, Type windowType, int width, int height);
        public void Show(System.ComponentModel.INotifyPropertyChanged ownerViewModel, IModalDialogViewModel viewModel, int width, int height);
        #endregion
    }
}
