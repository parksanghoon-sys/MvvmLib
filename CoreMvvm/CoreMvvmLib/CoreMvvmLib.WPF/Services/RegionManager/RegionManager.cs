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
        public void RegisterAddView(Type type)
        {
            ViewTypeStore.RegisterAddView(type);
        }

        public void RegisterAddView<TView>() where TView : class
        {
            this.RegisterAddView(typeof(TView));
        }
        public void RegisterAddView(string regionName, Type type)
        {
            ViewTypeStore.RegisterAddView(type);
            ViewTypeStore.CreateView(type.Name);

            if(RegionStorage.IsCheckRegionExist(regionName))
            {
                var region = RegionStorage.GetRegion(regionName);
                region.SourceViewName = type.Name;                
            }
            else
            {
                RegionStorage.SetRegisterRegion(regionName, type);
            }
        }
        public void RegisterAddView<TView>(string regionName) where TView : class
        {
            this.RegisterAddView(regionName, typeof(TView));
        }
        public void NavigateView(string regionName, string viewName)
        {
            try
            {                
                var region = RegionStorage.GetRegion(regionName);
                region.SourceViewName = viewName;                
                region.NavigateView();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void NavigateView(string regionName, string viewName, INotifyPropertyChanged viewModel)
        {
            try
            {
                var region = RegionStorage.GetRegion(regionName);
                region.SourceViewName = viewName;                
                region.NavigateView(viewModel);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
       
    

    
    }
}
