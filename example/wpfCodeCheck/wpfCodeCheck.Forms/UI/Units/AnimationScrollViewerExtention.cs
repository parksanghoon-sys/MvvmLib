using System.Windows;
using System.Windows.Controls;

namespace wpfCodeCheck.Forms.UI.Units
{
    public class AnimationScrollViewerExtention : ScrollViewer
    {
        public double CurrentVerticalOffset
        {
            get { return (double)GetValue(CurrentVerticalOffsetProperty); }
            set { SetValue(CurrentVerticalOffsetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentVerticalOffset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentVerticalOffsetProperty =
            DependencyProperty.Register("CurrentVerticalOffset", typeof(double), typeof(AnimationScrollViewerExtention), new PropertyMetadata(new PropertyChangedCallback(OnVerticalChanged)));

        private static void OnVerticalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AnimationScrollViewerExtention viewer = d as AnimationScrollViewerExtention;
            viewer.ScrollToVerticalOffset((double)e.NewValue);
        }
    }
}