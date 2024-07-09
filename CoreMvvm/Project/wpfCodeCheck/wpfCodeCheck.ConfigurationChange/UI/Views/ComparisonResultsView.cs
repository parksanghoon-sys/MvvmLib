using System.Windows;
using System.Windows.Controls;

namespace wpfCodeCheck.ConfigurationChange.UI.Views
{
    public class ComparisonResultsView : ContentControl
    {
        static ComparisonResultsView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ComparisonResultsView), new FrameworkPropertyMetadata(typeof(ComparisonResultsView)));
        }
    }
}
