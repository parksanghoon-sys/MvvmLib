using CoreMvvmLib.Component.UI.Views;
using CoreMvvmLib.Core.Messenger;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using wpfCodeCheck.Domain.Enums;

namespace wpfCodeCheck.Component.UI.Views
{
    public class LoadingDialogView : DialogBaseView, IDisposable
    {  
        public double Diameter
        {
            get { return (double)GetValue(DiameterProperty); }
            set { SetValue(DiameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Diameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DiameterProperty =
            DependencyProperty.Register("Diameter", typeof(double), typeof(LoadingDialogView), new PropertyMetadata(100.0d));


        public double Thickness
        {
            get { return (double)GetValue(ThicknessProperty); }
            set { SetValue(ThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Thickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.Register("Thickness", typeof(double), typeof(LoadingDialogView), new PropertyMetadata(1.0d));


        public Brush Color
        {
            get { return (Brush)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Brush), typeof(LoadingDialogView), new PropertyMetadata(Brushes.Wheat));
        public PenLineCap Cap
        {
            get { return (PenLineCap)GetValue(CapProperty); }
            set { SetValue(CapProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Cap.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CapProperty =
            DependencyProperty.Register("Cap", typeof(PenLineCap), typeof(LoadingDialogView), new PropertyMetadata(PenLineCap.Flat));
        static LoadingDialogView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LoadingDialogView), new FrameworkPropertyMetadata(typeof(LoadingDialogView)));
        }
        public LoadingDialogView()
        {
            this.Loaded += (s, e) =>
            {
                WeakReferenceMessenger.Default.Send<EMainViewDimming>(EMainViewDimming.DIMMING);
            };
            this.Closed += (s, e) =>
            {
                WeakReferenceMessenger.Default.Send<EMainViewDimming>(EMainViewDimming.NONE);                
                loadingTimer = null;
            };
        }
        TextBlock? text;
        int loading = 0;
        DispatcherTimer? loadingTimer;
        public override void OnApplyTemplate()
        {
            text = GetTemplateChild("PART_Loading") as TextBlock;
            
            if (text != null)
            {
                loadingTimer = new DispatcherTimer();

                // 1초 마다 Tick 됩니다.
                loadingTimer.Interval = TimeSpan.FromMilliseconds(1000);

                // Event 특성상 여러 이벤트를 등록시킬 수 있습니다.
                loadingTimer.Tick += (s, e) =>
                {
                    loading += 1;
                    text.Text = $"Loading {string.Join("", Enumerable.Repeat(".", loading))}";

                    if (loading == 5)
                    {
                        loading = 0;
                    }
                };

                loadingTimer.Start();
            }
            base.OnApplyTemplate();
        }

    }
}
