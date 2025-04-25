using CoreMvvmLib.Core.Services.DialogService;
using CoreMvvmLib.Core.Services.RegionManager;

namespace CoreMvvmLib.WPF.Services;

public partial class ServiceLocator
{
    private static IDialogService _dialogService = null;
    public static IDialogService DialogService
    {
        get
        {
            if(_dialogService is  null)
                _dialogService = new DialogService();

            return _dialogService;
        }
    }
    private static IRegionManager regionManager = null;
    public static IRegionManager RegionManager
    {
        get
        {
            if (regionManager == null)
                regionManager = new RegionManager();

            return regionManager;
        }
    }
}
