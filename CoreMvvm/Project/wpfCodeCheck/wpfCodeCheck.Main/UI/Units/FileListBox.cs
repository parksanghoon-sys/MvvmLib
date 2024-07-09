using System.Windows.Controls;
using System.Windows;

namespace wpfCodeCheck.Main.UI.Units
{
    public class FileListBox : ListBox
    {
        static FileListBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FileListBox), new FrameworkPropertyMetadata(typeof(FileListBox)));
        }
        public FileListBox()
        {
            
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new FileListItem();
        }
    }
}
