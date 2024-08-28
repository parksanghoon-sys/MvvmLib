using System.Windows.Controls;
using System.Windows;

namespace wpfCodeCheck.ProjectChangeTracker.UI.Units
{
    public class FailFileListBox : ListBox
    {
        static FailFileListBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FailFileListBox), new FrameworkPropertyMetadata(typeof(FailFileListBox)));
        }      

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new FailFileListItem();
        }
    }
}
