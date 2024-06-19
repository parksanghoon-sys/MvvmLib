using CoreMvvmLib.Component.UI.Views;
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
