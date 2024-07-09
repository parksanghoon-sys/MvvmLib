using System.Windows.Controls;
using System.Windows;

namespace CoreMvvmLib.Component.UI.Units
{
    public class CustomStackPannel : StackPanel
    {
        public double ChildSpacing
        {
            get { return (double)GetValue(ChildSpacingProperty); }
            set { SetValue(ChildSpacingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChildSpacing.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChildSpacingProperty =
            DependencyProperty.Register("ChildSpacing", typeof(double), typeof(CustomStackPannel), new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.AffectsArrange));
        protected override Size MeasureOverride(Size availableSize)
        {
            var num = 0.0;
            var num2 = 0.0;
            var num3 = 0.0;
            var num4 = 0.0;

            foreach (UIElement element in base.InternalChildren)
            {
                if (element != null)
                {
                    element.Measure(availableSize);
                    Size desiredSize = element.DesiredSize;
                    if (base.Orientation == Orientation.Vertical)
                    {
                        num4 += desiredSize.Height + ChildSpacing;
                        num = Math.Max(num, desiredSize.Width);
                    }
                    else
                    {
                        num3 += desiredSize.Width + ChildSpacing;
                        num2 = Math.Max(num, desiredSize.Height);
                    }
                }
            }
            if (base.InternalChildren.Count > 0)
            {
                if (base.Orientation == Orientation.Vertical)
                {
                    num4 -= ChildSpacing;
                }
                else
                {
                    num3 -= ChildSpacing;
                }
            }
            return (base.Orientation == Orientation.Vertical) ? new Size(num, num4) : new Size(num3, num2);
        }
        protected override Size ArrangeOverride(Size finalSize)
        {
            double num = 0.0;
            foreach (UIElement internalChild in base.InternalChildren)
            {
                if (internalChild != null)
                {
                    Size desiredSize = internalChild.DesiredSize;
                    if (base.Orientation == Orientation.Vertical)
                    {
                        internalChild.Arrange(new Rect(0.0, num, finalSize.Width, desiredSize.Height));
                        num += desiredSize.Height + ChildSpacing;
                    }
                    else
                    {
                        internalChild.Arrange(new Rect(num, 0.0, desiredSize.Width, finalSize.Height));
                        num += desiredSize.Width + ChildSpacing;
                    }
                }
            }

            if (base.InternalChildren.Count > 0)
            {
                num -= ChildSpacing;
            }

            return (base.Orientation == Orientation.Vertical) ? new Size(finalSize.Width, num) : new Size(num, finalSize.Height);
        }
    }
}
