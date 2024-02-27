using System.Windows;

namespace CoreMvvmLib.WPF.Services.RegionManager
{
    internal class ViewTypeStore
    {
        public static Dictionary<string, Type> _viewTypes = new Dictionary<string, Type>();
        public static Dictionary<string, FrameworkElement> _views = new Dictionary<string, FrameworkElement>();

        public static void RegisterAddView(Type type)
        {
            if(_viewTypes.ContainsKey(type.Name)) 
            { 
                _viewTypes.Add(type.Name, type);
            }
        }
        public static Type GetView(string name)
        {
            if(_viewTypes.ContainsKey(name) == false)
            {
                return null;
            }
            return _viewTypes[name];
        }
        public static FrameworkElement CreateView(string name)
        {
            var type = GetView(name);
            if(type == null)
            {
                throw (new Exception("등록된 View가 없습니다."));
            }
            if(_views.ContainsKey(name) == null)
            {
                var view = (FrameworkElement)Activator.CreateInstance(type);
                _views.Add(name, view);
            }
            return _views[name];
        }
    }
}
