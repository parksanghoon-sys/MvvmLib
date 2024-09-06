using System.Windows;
using System.Windows.Controls;

namespace wpfCodeCheck.SDDExport.UI.Views
{
    public class SddExprotView : ContentControl
    {
        static SddExprotView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SddExprotView), new FrameworkPropertyMetadata(typeof(SddExprotView)));
        }
    }
}
