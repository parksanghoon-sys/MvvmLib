using System.Windows;
using System.Windows.Controls;

namespace wpfCodeCheck.Component.UI.Units
{
    public class DarkTextBox : TextBox
    {
        static DarkTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DarkTextBox), new FrameworkPropertyMetadata(typeof(DarkTextBox)));
        }
    }
}
