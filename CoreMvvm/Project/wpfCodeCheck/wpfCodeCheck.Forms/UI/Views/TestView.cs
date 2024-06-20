using CoreMvvmLib.Component.UI.Views;
using System.Windows;
using System.Windows.Controls;

namespace wpfCodeCheck.Forms.UI.Views
{
    public class TestView : ContentControl
    {
        static TestView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TestView), new FrameworkPropertyMetadata(typeof(TestView)));
        }
    }
}
