using System.Windows;
using System.Windows.Controls;

namespace CoreMvvmLib.Component.UI.Units
{
    public class MaximizeButton : Button
    {
        public static readonly DependencyProperty IsMaximizeProperty =
            DependencyProperty.Register("IsMaximize", typeof(bool), typeof(MaximizeButton), new PropertyMetadata(false, MaximizePropertyChanged));
        public bool IsMaximize
        {
            get { return (bool)GetValue(IsMaximizeProperty); }
            set { SetValue(IsMaximizeProperty, value); }
        }        

        private static void MaximizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var btn = (MaximizeButton)d;
            btn.icon.Icon = btn.IsMaximize ? IconType.Restore : IconType.Maximize;
        }
        private ImageIcon icon;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (GetTemplateChild("PART_IMG") is ImageIcon maxbtn)
            {
                icon = maxbtn;
            }
        }

        static MaximizeButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MaximizeButton), new FrameworkPropertyMetadata(typeof(MaximizeButton)));
        }
    }
}
