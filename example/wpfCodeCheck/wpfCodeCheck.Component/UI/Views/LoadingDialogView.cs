using CoreMvvmLib.Component.UI.Views;
using CoreMvvmLib.Core.Messenger;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using wpfCodeCheck.Domain.Enums;

namespace wpfCodeCheck.Component.UI.Views
{
    public class LoadingDialogView : DialogBaseView, IDisposable
    {
        public string StatusMessage
        {
            get { return (string)GetValue(StatusMessageProperty); }
            set { SetValue(StatusMessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StatusMessage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusMessageProperty =
            DependencyProperty.Register("StatusMessage", typeof(string), typeof(LoadingDialogView), new PropertyMetadata(""));


        public int ScanProgress
        {
            get { return (int)GetValue(ScanProgressProperty); }
            set { SetValue(ScanProgressProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScanProgress.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScanProgressProperty =
            DependencyProperty.Register("ScanProgress", typeof(int), typeof(LoadingDialogView), new PropertyMetadata(0));

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
            this.IsVisibleChanged += (s, e) =>
            {
                if (this.Visibility == Visibility.Hidden)
                {
                    WeakReferenceMessenger.Default.Send<EMainViewDimming>(EMainViewDimming.NONE);
                    WeakReferenceMessenger.Default.UnRegister<LoadingDialogView, (int, string)>(this, OnReceiveStatus);
                    loadingTimer = null;
                }
                else
                {
                    WeakReferenceMessenger.Default.Send<EMainViewDimming>(EMainViewDimming.DIMMING);
                    WeakReferenceMessenger.Default.Register<LoadingDialogView, (int, string)>(this, OnReceiveStatus);
                }
            };
        }
        private object _lock = new();

        private void OnReceiveStatus(LoadingDialogView view, (int, string) status)
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                StatusMessage = status.Item2;
                ScanProgress = status.Item1;
                if (statusText is not null)
                    statusText.Text = StatusMessage;
            });

        }

        TextBlock? loadingTxt;
        TextBlock? statusText;

        DispatcherTimer? loadingTimer;
        public override void OnApplyTemplate()
        {
            loadingTxt = GetTemplateChild("PART_Loading") as TextBlock;
            statusText = GetTemplateChild("PART_Status") as TextBlock;

            if (loadingTxt != null)
            {
                loadingTimer = new DispatcherTimer();

                // 1초 마다 Tick 됩니다.
                loadingTimer.Interval = TimeSpan.FromMilliseconds(1000);
                int loading = 0;
                // Event 특성상 여러 이벤트를 등록시킬 수 있습니다.
                loadingTimer.Tick += (s, e) =>
                {
                    loading += 1;
                    loadingTxt.Text = $"Loading {$"{ScanProgress}%"}{string.Join("", Enumerable.Repeat(".", loading))}";
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
