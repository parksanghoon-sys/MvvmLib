using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace wpfCodeCheck.Forms.UI.Units
{
    public class TabContent : ContentControl
    {
        static TabContent()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabContent), new FrameworkPropertyMetadata(typeof(TabContent)));
        }
        private AnimationScrollViewerExtention contentScroll;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            contentScroll = GetTemplateChild("PART_ContentScroll") as AnimationScrollViewerExtention;
        }
        public TabContent()
        {
            this.DataContextChanged += TabContent_DataContextChanged;
        }

        private void TabContent_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ModeScroll();
        }

        public void ModeScroll()
        {

            if (contentScroll != null)
            {
                // Define the target vertical offset
                double targetVerticalOffset = 0; // Set this to your desired target offset

                // Create the DoubleAnimation
                DoubleAnimation verticalOffsetAnimation = new DoubleAnimation
                {
                    From = contentScroll.VerticalOffset,
                    To = targetVerticalOffset,
                    Duration = new Duration(TimeSpan.FromMilliseconds(500)),
                    EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseIn }
                };

                // Create a Storyboard to contain the animation
                Storyboard sb = new Storyboard();

                // Use the custom AnimationScrollViewerExtension.CurrentVerticalOffsetProperty
                Storyboard.SetTarget(verticalOffsetAnimation, contentScroll);
                Storyboard.SetTargetProperty(verticalOffsetAnimation, new PropertyPath(AnimationScrollViewerExtention.CurrentVerticalOffsetProperty));

                // Add the animation to the Storyboard
                sb.Children.Add(verticalOffsetAnimation);

                // Begin the animation
                sb.Begin();
            }
        }
    }
}
