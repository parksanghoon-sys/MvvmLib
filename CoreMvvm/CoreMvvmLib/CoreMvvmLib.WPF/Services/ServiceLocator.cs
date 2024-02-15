using CoreMvvmLib.Core.Services.DialogService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
