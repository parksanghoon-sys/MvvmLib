using System.Windows;
using System.Windows.Controls;

namespace wpfCodeCheck.ProjectChangeTracker.UI.Views
{
    public class ComparisonResultsView : ContentControl
    {
        static ComparisonResultsView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ComparisonResultsView), new FrameworkPropertyMetadata(typeof(ComparisonResultsView)));
        }
    }
}
