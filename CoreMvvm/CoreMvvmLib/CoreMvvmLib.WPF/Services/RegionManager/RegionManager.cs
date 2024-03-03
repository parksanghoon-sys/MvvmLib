using CoreMvvmLib.Core.Services.RegionManager;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace CoreMvvmLib.WPF.Services.RegionManager
{
    public class RegionManager : IRegionManager
    {
        #region Dependency Property
        public static DependencyProperty RegionNameProperty = DependencyProperty.RegisterAttached("RegionName", typeof(string), typeof(RegionManager), new PropertyMetadata("", OnRegionNameChanged));

    
        #endregion
        #region Static Function
        private static bool IsInDesignMode(DependencyObject element)
        {
            return DesignerProperties.GetIsInDesignMode(element);
        }
        #endregion

        #region Event Handler
        public static string GetRegionName(DependencyObject obj)
        {
            return (string)obj.GetValue(RegionNameProperty);
        }

        public static void SetRegionName(DependencyObject obj, string value)
        {
            obj.SetValue(RegionNameProperty, value);
        }
        private static void OnRegionNameChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
        {
            if(IsInDesignMode(element) == false)
            {
                var regionKey = GetRegionName(element);
                var contentControl = element as ContentControl;
                if(contentControl is not null)
                {
                    if((string)args.NewValue != "")
                    {
                        try
                        {
                            if(RegionStorage.IsCheckRegionExist(regionKey) == false)
                            {
                                RegionStorage.SetRegisterRegion(regionKey);
                            }
                            var region = RegionStorage.GetRegion(regionKey);
                            region.Sources.Add(new WeakReference(contentControl));
                            region.OnContentUpdate -= OnContentUpdate;
                            region.OnContentUpdate += OnContentUpdate;

                            region.NavigateView();
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }
                    }
                }
            }
        }

        private static void OnContentUpdate(object arg1, object arg2)
        {
            var contentConrol = arg1 as ContentControl;

            if (contentConrol is null)
                return;
            contentConrol.Content = arg2;
        }
        #endregion
        public void NavigateView(string regionName, string viewName)
        {
            throw new NotImplementedException();
        }

        public void NavigateView(string regionName, string viewName, INotifyPropertyChanged viewModel)
        {
            throw new NotImplementedException();
        }

        public void RegisterAddView(Type type)
        {
            throw new NotImplementedException();
        }

        public void RegisterAddView<TView>() where TView : class
        {
            throw new NotImplementedException();
        }

        public void RegisterAddView(string regionName, Type type)
        {
            throw new NotImplementedException();
        }

        public void RegisterAddView<TView>(string regionName) where TView : class
        {
            throw new NotImplementedException();
        }
    }
}
