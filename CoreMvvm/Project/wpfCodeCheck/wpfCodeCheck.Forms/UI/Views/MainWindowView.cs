using CoreMvvmLib.Component.UI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace wpfCodeCheck.Forms.UI.Views
{
    public class MainWindowView : DarkThemeWindow
    {
        static MainWindowView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MainWindowView), new FrameworkPropertyMetadata(typeof(MainWindowView)));
        }
    }
}
