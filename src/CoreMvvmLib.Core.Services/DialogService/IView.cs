namespace CoreMvvmLib.Core.Services.DialogService
{
    public interface IView
    {
        object Sourse { get; }
        object DataContext { get; }
        bool IsAlive { get; }
        object GetOwner();
    }
}
