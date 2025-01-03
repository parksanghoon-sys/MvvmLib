using CoreMvvmLib.WPF.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CoreMvvmLib.WPF.Hosting
{
    public abstract class GenericHostApplication : Application
    {
        private CoreDumpHelper.MiniDumpType _dumpType = CoreDumpHelper.MiniDumpType.MiniDumpNormal;
        private bool _canGenerateDump = false;

        protected GenericHostApplication()
        {
            
        }
        protected void SetDumpOption(bool canGenerateDump, CoreDumpHelper.MiniDumpType dumpType)
        {
            _canGenerateDump = canGenerateDump;
            _dumpType = dumpType;
        }
    }
}
