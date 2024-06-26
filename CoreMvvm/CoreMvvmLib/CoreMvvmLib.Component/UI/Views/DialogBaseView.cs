using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CoreMvvmLib.Component.UI.Views
{
    public class DialogBaseView : Window , IDisposable
    {
        public DialogBaseView()
        {
            this.AllowsTransparency = true;
            this.WindowStyle = WindowStyle.None;
        }
        private bool _isDisposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing)
            {

            }
            
            _isDisposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~DialogBaseView()
        {
            Dispose(false);
        }
    }
}
