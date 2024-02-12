using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
