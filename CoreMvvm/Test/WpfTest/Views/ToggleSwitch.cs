using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfTest1.Views
{
    public class ToggleSwitch : ToggleButton
    {
        static ToggleSwitch()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToggleSwitch), new FrameworkPropertyMetadata(typeof(ToggleSwitch)));
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (IsChecked == true)
            {
                var ellipse = Template.FindName("EllipseToggle", this) as Ellipse;
                var border = Template.FindName("BorderToggle", this) as Border;

                //ellipse.SetValue(Canvas.LeftProperty, 36.0);
                border.SetValue(Border.BackgroundProperty, new SolidColorBrush(Color.FromRgb(0x66, 0xCA, 0xB9)));
            }
        }
    }
}
