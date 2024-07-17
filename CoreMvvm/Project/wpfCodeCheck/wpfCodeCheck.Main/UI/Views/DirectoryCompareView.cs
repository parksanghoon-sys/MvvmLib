using System.Windows.Controls;
using System.Windows;
using wpfCodeCheck.Main.UI.Units;

namespace wpfCodeCheck.Main.UI.Views
{
    public class DirectoryCompareView : ContentControl
    {
        static DirectoryCompareView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DirectoryCompareView), new FrameworkPropertyMetadata(typeof(DirectoryCompareView)));
        }    
    }
}
