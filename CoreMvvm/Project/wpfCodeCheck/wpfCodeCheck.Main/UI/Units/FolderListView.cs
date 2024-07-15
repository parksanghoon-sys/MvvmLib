using System.Windows;
using System.Windows.Controls;

namespace wpfCodeCheck.Main.UI.Units
{
    internal class FolderListView : ContentControl
    {
        public string DirectoryPath
        {
            get { return (string)GetValue(DirectoryPathProperty); }
            set { SetValue(DirectoryPathProperty, value); }
        }
        public static readonly DependencyProperty DirectoryPathProperty =
            DependencyProperty.Register("DirectoryPath", typeof(string), typeof(FolderListView), new PropertyMetadata(""));


        static FolderListView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FolderListView), new FrameworkPropertyMetadata(typeof(FolderListView)));
        }
    }
}
