using CoreMvvmLib.Core.IOC;
using CoreMvvmLib.Core.Services.DialogService;
using System.ComponentModel;
using System.Windows;

namespace CoreMvvmLib.WPF.Services;

internal class DialogService : IDialogService
{
    private IServiceContainer _serviceContainer;  
    protected virtual Window FindOwnerWindow(INotifyPropertyChanged viewModel)
    {
       
         foreach (Window? window in Application.Current.Windows)
        {
            if (window == null || viewModel.Equals(window.DataContext) == false)
                continue;
            try
            {
                return window;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                break;
            }
        };
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
    public void RegisterDialog<TWindow>() where TWindow : class
    {
        var type = typeof(TWindow);
        if (!type.IsSubclassOf(typeof(Window)))
            throw new ArgumentException(nameof(type));

        this.RegisterDialog(typeof(TWindow));
    }

    public void RegisterDialog(Type type, bool isSingle)
    {
        DialogStorage.RegisterDialog(type, isSingle);
    }

    public void RegisterSingleDialog<TWindow>(bool isSingle = false) where TWindow : class
    {
        var type = typeof(TWindow);
        if (!type.IsSubclassOf(typeof(Window)))
            throw new ArgumentException(nameof(type));

        this.RegisterDialog(typeof(TWindow), isSingle);
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
    public bool Activate(Type type)
    {
        var dialogConatiner = DialogStorage._dialogTypes[type];
        return dialogConatiner.IsActivate;
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
    public bool Close(Type dialogType)
    {
        bool chekck = false;

        var dialogConatiner = DialogStorage._dialogTypes[dialogType];
        if (dialogConatiner.IsActivate == true)
        {
            if (dialogConatiner.IsOnlySingle == true)
            {
                if(dialogConatiner.CallCount == 1)
                {
                    dialogConatiner.Window.Hide();
                    chekck = true;
                }                    
                dialogConatiner.CallCount--;
            }
            else
            {
                dialogConatiner.Window.Close();    
                chekck = true;
            }
        }
              
        return chekck;
    }

   
    public bool ShowDialog(INotifyPropertyChanged ownerViewModel, Type windwoType, string title, int width, int height)
    {
        var dialogContainer = DialogStorage.CreateDialog(windwoType);
        var name = windwoType.Name.Replace("View", "ViewModel");
        var viewModelType = _serviceContainer.TypeGet(name);
        if (viewModelType != null)
        {
            var viewModel = _serviceContainer.GetService(viewModelType) as IModalDialogViewModel;
            viewModel.Title = title;
            dialogContainer.Window.DataContext = viewModel;
        }
      
        var onwerWindow = this.FindOwnerWindow(ownerViewModel);
        dialogContainer.Window.Name = windwoType.Name;
        dialogContainer.Window.Owner = onwerWindow;
        dialogContainer.Window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        dialogContainer.Window.Width = width;
        dialogContainer.Window.Height = height;
        if(dialogContainer.IsOnlySingle)
        {
            dialogContainer.CallCount++;
            dialogContainer.Window.ShowDialog();
        }
        dialogContainer.Window.ShowDialog();

        return true;
    }

    public bool ShowDialog(INotifyPropertyChanged ownerViewModel, IModalDialogViewModel viewModel, string title, int width, int height)
    {
        var viewModelType = viewModel.GetType();
        var name = viewModelType.Name.Replace("ViewModel", "View");
        var dialogContainer = DialogStorage.CreateDialog(Type.GetType(name));

        dialogContainer.Window.DataContext = viewModel;
        var ownerWindow = this.FindOwnerWindow(ownerViewModel);

        dialogContainer.Window.Owner = ownerWindow;
        dialogContainer.Window.Width = width;
        dialogContainer.Window.Height = height;

        if (dialogContainer.IsOnlySingle)
        {
            dialogContainer.CallCount++;
            dialogContainer.Window.ShowDialog();
        }
        dialogContainer.Window.ShowDialog();


        return viewModel.DialogResult;
    }
    // TODO : Dialog 제어 관련 추후 다른 방안 생각해봄
    public void Show(INotifyPropertyChanged ownerViewModel, Type windowType, int width, int height)
    {
        var dialogContainer = DialogStorage.CreateDialog(windowType);
        var name = windowType.Name.Replace("View", "ViewModel");
        var viewModelType = _serviceContainer.TypeGet(name);

        if (viewModelType != null)
        {
            var viewModel = _serviceContainer.GetService(viewModelType) as IModalDialogViewModel;
            dialogContainer.Window.DataContext = viewModel;
        }
        var onwerWindow = this.FindOwnerWindow(ownerViewModel);
        dialogContainer.Window.Owner = Application.Current.MainWindow;
        dialogContainer.Window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        dialogContainer.Window.Width = width;
        dialogContainer.Window.Height = height;
        dialogContainer.Window.Name = windowType.Name;

        if (dialogContainer.IsOnlySingle)
        {
            if (dialogContainer.CallCount == 1)
            {
                dialogContainer.Window.Show();
            }             
        }
        else
        {
            dialogContainer.Window.Show();
        }        
    }

    public void Show(INotifyPropertyChanged ownerViewModel, IModalDialogViewModel viewModel, int width, int height)
    {
        var viewModelType = viewModel.GetType();
        var name = viewModelType.Name.Replace("ViewModel", "View");
        var conatiner = DialogStorage.CreateDialog(Type.GetType(name));

        conatiner.Window.DataContext = viewModel;
        var ownerWindow = this.FindOwnerWindow(ownerViewModel);

        conatiner.Window.Owner = ownerWindow;
        conatiner.Window.Width = width;
        conatiner.Window.Height = height;
        conatiner.Window.Show();
    }

   
}
