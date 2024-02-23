using CoreMvvmLib.Core.Services.DialogService;

namespace CoreMvvmLib.WPF.Services
{
    public partial class ServiceLocator
    {
        private static IDialogService _dialogService = null;
        public static IDialogService DialogService
        {
            get
            {
                if(_dialogService is  null)
                    _dialogService = new DialogService.DialogService();

                return _dialogService;
            }
        }
    }
}
