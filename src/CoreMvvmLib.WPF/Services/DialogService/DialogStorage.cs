using System.Windows;
using IsSingleton = bool;
namespace CoreMvvmLib.WPF.Services;

internal class DialogStorage
{
    internal static Dictionary<Type, DialogConatiner> _dialogTypes = new Dictionary<Type, DialogConatiner>();
    
    internal static void RegisterDialog(Type type, bool isSingle = false)
    {
        var container = new DialogConatiner
        {
            Type = type,
            CallCount = 0,
            IsOnlySingle = isSingle
        };
        if (_dialogTypes.ContainsKey(type) == false)
        {
            _dialogTypes.Add(type, container);            
        }
    }
    internal static DialogConatiner Dialog(Type type)
    {
        if (_dialogTypes.ContainsKey(type) == true)
            return _dialogTypes[type];
        return null;
    }
    internal static DialogConatiner CreateDialog(Type type)
    {
        var item = DialogStorage.Dialog(type);
        if (item.Type is null)
        {
            return null;
        }
        if(item.IsOnlySingle == false)
        {
            _dialogTypes[type].IsActivate = true;
            _dialogTypes[type].Window = (Window)Activator.CreateInstance(item.Type);
            
            return _dialogTypes[type];
        }            
        else
        {
            if (_dialogTypes[type].IsActivate is false && _dialogTypes[type].CallCount == 0)
            {
                _dialogTypes[type].Window = (Window)Activator.CreateInstance(item.Type);

                _dialogTypes[type].CallCount++;
                _dialogTypes[type].IsActivate = true;
            }
            else
            {
                _dialogTypes[type].CallCount++;
            }
            
            return _dialogTypes[type];
        }                    
    }
}
internal record DialogConatiner
{
    public Type? Type { get; set; }
    public bool IsOnlySingle { get; set; }
    public int CallCount { get; set; }
    public bool IsActivate { get; set; }
    public Window Window { get; set; }
}

