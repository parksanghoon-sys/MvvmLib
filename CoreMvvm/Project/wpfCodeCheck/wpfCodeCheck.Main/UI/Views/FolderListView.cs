using System.Windows;
using System.Windows.Controls;

namespace wpfCodeCheck.Main.UI.Views
{
    internal class FolderListView : ContentControl
    {
        static FolderListView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FolderListView), new FrameworkPropertyMetadata(typeof(FolderListView)));
        }
    }
}
