using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreMvvmLib.Core.Services.DialogService
{
    public interface IModalDialogViewModel
    {
        public bool DialogResult { get; }
        public string Title { get; set; }
    }
}
