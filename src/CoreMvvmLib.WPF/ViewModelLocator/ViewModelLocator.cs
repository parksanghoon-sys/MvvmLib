
using CoreMvvmLib.Core.IOC;
using System.ComponentModel;
using System.Windows;

namespace CoreMvvmLib.WPF
{
    public class ViewModelLocator
    {
        private static IServiceContainer _serviceContainer = null;

        public static DependencyProperty AutoWireViewModelProperty =
            DependencyProperty.RegisterAttached("AutoWireViewModel", typeof(bool), typeof(ViewModelLocator), new PropertyMetadata(false, OnAutoWireViewModelChanged));

        #region Public Static Method
        public static void SetServiceProvider(IServiceContainer serviceContainer)
        {
            _serviceContainer = serviceContainer;
        }
        public static bool GetAutoWireViewMoel(DependencyObject dependencyObject)
        {
            return (bool)dependencyObject.GetValue(AutoWireViewModelProperty);
        }
        public static void SetAutoWireViewModel(DependencyObject dependencyObject, bool value)
        {
            dependencyObject.SetValue(AutoWireViewModelProperty, value);
        }
        #endregion
        private static bool IsIndesignMode(DependencyObject dependencyObject)
        {
            return DesignerProperties.GetIsInDesignMode(dependencyObject);
        }
        private static void OnAutoWireViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(IsIndesignMode(d) == false)
            {
                var frameworkElement = d as FrameworkElement;
                if (frameworkElement != null)
                {
                    if ((bool)e.NewValue == true)
                        frameworkElement.Initialized += FrameworkElement_Initialized;
                    else
                        frameworkElement.Initialized -= FrameworkElement_Initialized;
                }
            }
        }

        private static void FrameworkElement_Initialized(object? sender, EventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            var typeName = frameworkElement.GetType().Name;
            var viewModelName = typeName.Replace("View", "ViewModel");

            var type = _serviceContainer.TypeGet(viewModelName);
            var vm = _serviceContainer.GetService(type);
            frameworkElement.DataContext = vm;

        }
    }
}
