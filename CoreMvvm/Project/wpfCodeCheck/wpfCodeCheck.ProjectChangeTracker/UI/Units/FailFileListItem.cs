using System.Windows.Controls;
using System.Windows;

namespace wpfCodeCheck.ProjectChangeTracker.UI.Units
{
    public class FailFileListItem : ListBoxItem
    {
        static FailFileListItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FailFileListItem), new FrameworkPropertyMetadata(typeof(FailFileListItem)));
        }
    }
}
