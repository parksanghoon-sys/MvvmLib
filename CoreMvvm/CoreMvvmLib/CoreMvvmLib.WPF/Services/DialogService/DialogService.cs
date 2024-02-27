using CoreMvvmLib.Core.IOC;
using CoreMvvmLib.Core.Services.DialogService;
using System.ComponentModel;
using System.Windows;

namespace CoreMvvmLib.WPF.Services.DialogService
{
    internal class DialogService : IDialogService
    {
        private IServiceContainer _serviceContainer;
        public DialogService()
        {
            
        }
        protected virtual Window FindOwnerWindow(INotifyPropertyChanged viewModel)
        {
            return null;
        }
        public void SetServiceProvider(IServiceContainer serviceProvider)
        {
            _serviceContainer = serviceProvider;
        }
        public void RegisterDialog(Type type)
        {
            DialogStorage.RegisterDialog(type);
        }
        public bool Activate(INotifyPropertyChanged viewModel)
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));
            Window? windowToActivate =
                (
                    from Window? window in Application.Current.Windows
                    where window != null
                    where viewModel.Equals(window.DataContext)
                    select window
                )
                .FirstOrDefault();

            return windowToActivate?.Activate() ?? false;
        }

        public bool Close(INotifyPropertyChanged viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

            bool chekck = false;
            foreach (Window? window in Application.Current.Windows)
            {
                if (window == null || viewModel.Equals(window.DataContext) == false)
                    continue;
                try
                {
                    window.Close();
                    chekck = true;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                    break;
                }
            }
            return chekck;
        }


        public void RegisterDialog<TWindow>() where TWindow : class
        {
            var type = typeof(TWindow);
            if (!type.IsSubclassOf(typeof(Window)))
                throw new ArgumentException(nameof(type));

            this.RegisterDialog(typeof(TWindow));
        }
    
        public bool ShowDialog(INotifyPropertyChanged ownerViewModel, string windowName, string title, int width, int height)
        {
            var window = DialogStorage.CreateDialog(windowName);
            var name = windowName.Replace("View", "ViewModel");
            var viewModelType = _serviceContainer.TypeGet(name);
            var viewModel = _serviceContainer.GetService(viewModelType) as IModalDialogViewModel;
            viewModel.Title = title;

            window.DataContext = viewModel;
            var onwerWindow = this.FindOwnerWindow(ownerViewModel);
            window.Owner = onwerWindow;
            window.Width = width;
            window.Height = height;
            window.ShowDialog();

            return viewModel.DialogResult;
        }

        public bool ShowDialog(INotifyPropertyChanged ownerViewModel, IModalDialogViewModel viewModel, string title, int width, int height)
        {
            var viewModelName = viewModel.GetType().Name;
            var name = viewModelName.Replace("ViewModel", "View");
            Window window = DialogStorage.CreateDialog(name);

            window.DataContext = viewModel;
            var ownerWindow = this.FindOwnerWindow(ownerViewModel);

            window.Owner = ownerWindow;
            window.Width = width;
            window.Height = height;
            window.ShowDialog();


            return viewModel.DialogResult;
        }
        public void Show(INotifyPropertyChanged ownerViewModel, string windowName, int width, int height)
        {
            var window = DialogStorage.CreateDialog(windowName);
            var name = windowName.Replace("View", "ViewModel");
            var viewModelType = _serviceContainer.TypeGet(name);
            var viewModel = _serviceContainer.GetService(viewModelType) as IModalDialogViewModel;            

            window.DataContext = viewModel;
            var onwerWindow = this.FindOwnerWindow(ownerViewModel);
            window.Owner = onwerWindow;
            window.Width = width;
            window.Height = height;
            window.Show();
            
        }

        public void Show(INotifyPropertyChanged ownerViewModel, IModalDialogViewModel viewModel, int width, int height)
        {
            var viewModelName = viewModel.GetType().Name;
            var name = viewModelName.Replace("ViewModel", "View");
            Window window = DialogStorage.CreateDialog(name);

            window.DataContext = viewModel;
            var ownerWindow = this.FindOwnerWindow(ownerViewModel);

            window.Owner = ownerWindow;
            window.Width = width;
            window.Height = height;
            window.Show();            
        }
    }
}
