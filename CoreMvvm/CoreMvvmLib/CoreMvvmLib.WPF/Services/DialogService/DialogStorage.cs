using System.Windows;

namespace CoreMvvmLib.WPF.Services;

internal class DialogStorage
{
    internal static Dictionary<string, Type> _dialogTypes = new Dictionary<string, Type>();
    internal static void RegisterDialog(Type type)
    {
        if(_dialogTypes.ContainsKey(type.Name) == false)
        {
            _dialogTypes.Add(type.Name, type);
        }
    }
    internal static Type Dialog(string name)
    {
        if(_dialogTypes.ContainsKey(name) == true)
            return _dialogTypes[name];
        return null;
    }
    internal static Window CreateDialog(string name)
    {
        var type = DialogStorage.Dialog(name);
        if (type is null)
        {
            return null;
        }

        var windwow = (Window)Activator.CreateInstance(type);
        return windwow;
    }
}
