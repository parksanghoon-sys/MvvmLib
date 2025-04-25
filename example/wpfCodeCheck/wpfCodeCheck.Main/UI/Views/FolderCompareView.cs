using System.Windows;
using System.Windows.Controls;

namespace wpfCodeCheck.Main.UI.Views
{
    public class FolderCompareView : ContentControl
    {
        static FolderCompareView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FolderCompareView), new FrameworkPropertyMetadata(typeof(FolderCompareView)));
        }
    }
}
