using System.Windows;
using System.Windows.Controls;

namespace wpfCodeCheck.SDDExport.UI.Views
{
    public class SddExportView : ContentControl
    {
        static SddExportView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SddExportView), new FrameworkPropertyMetadata(typeof(SddExportView)));
        }
    }
}
