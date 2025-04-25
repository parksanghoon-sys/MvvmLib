using CoreMvvmLib.WPF.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace CoreMvvmLib.WPF.Hosting
{
    public abstract class GenericHostApplication : Application
    {
        private CoreDumpHelper.MiniDumpType _dumpType = CoreDumpHelper.MiniDumpType.MiniDumpNormal;
        private bool _canGenerateDump = false;

        protected GenericHostApplication()
        {
            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            Dispatcher.UnhandledExceptionFilter += Dispatcher_UnhandledExceptionFilter;
        }

        private void Dispatcher_UnhandledExceptionFilter(object sender, DispatcherUnhandledExceptionFilterEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected void SetDumpOption(bool canGenerateDump, CoreDumpHelper.MiniDumpType dumpType)
        {
            _canGenerateDump = canGenerateDump;
            _dumpType = dumpType;
        }
    }
}
