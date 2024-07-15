using CoreMvvmLib.Component.UI.Views;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace wpfCodeCheck.Shared.UI.Views
{
    public class CircularProgressBarView : DialogBaseView
    {
        static CircularProgressBarView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CircularProgressBarView), new FrameworkPropertyMetadata(typeof(CircularProgressBarView)));
        }
     

        public static readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register("Progress", typeof(double), typeof(CircularProgressBarView), new PropertyMetadata(0.0, OnProgressChanged));

        public double Progress
        {
            get { return (double)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }

        private static void OnProgressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as CircularProgressBarView;
            control.InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            double angle = 360 * Progress / 100;
            Point center = new Point(ActualWidth / 2, ActualHeight / 2);
            double radius = Math.Min(ActualWidth, ActualHeight) / 2 - StrokeThickness / 2;

            PathFigure pathFigure = new PathFigure { StartPoint = center };

            bool isLargeArc = angle >= 180.0;

            Point endPoint = new Point(
                center.X + radius * Math.Cos(Math.PI * angle / 180),
                center.Y - radius * Math.Sin(Math.PI * angle / 180));

            ArcSegment arcSegment = new ArcSegment
            {
                Size = new Size(radius, radius),
                Point = endPoint,
                SweepDirection = SweepDirection.Clockwise,
                IsLargeArc = isLargeArc
            };

            PathSegmentCollection segments = new PathSegmentCollection();
            segments.Add(arcSegment);

            PathFigureCollection figures = new PathFigureCollection();
            pathFigure.Segments = segments;
            figures.Add(pathFigure);

            PathGeometry geometry = new PathGeometry();
            geometry.Figures = figures;

            drawingContext.DrawGeometry(null, new Pen(Stroke, StrokeThickness), geometry);
        }

        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(CircularProgressBarView), new PropertyMetadata(Brushes.Black));

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(CircularProgressBarView), new PropertyMetadata(5.0));
        
    }
}
