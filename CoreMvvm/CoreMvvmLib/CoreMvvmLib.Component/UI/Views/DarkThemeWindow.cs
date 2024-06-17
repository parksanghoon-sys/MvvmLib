﻿using CoreMvvmLib.Component.UI.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CoreMvvmLib.Component.UI.Views
{
    public class DarkThemeWindow : Window
    {
        public static readonly DependencyProperty PopupOpenProperty;
        public static readonly DependencyProperty TitleHeaderBackgroundProperty;
        public static readonly DependencyProperty CloseCommandProperty;
        public static readonly new DependencyProperty TitleProperty;
        public static readonly DependencyProperty IsShowTaskBarProperty;
        public new object Title { get => GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
        public ICommand CloseCommand { get => (ICommand)GetValue(CloseCommandProperty); set => SetValue(CloseCommandProperty, value); }
        public Brush TitleHeaderBackground { get => (Brush)GetValue(TitleHeaderBackgroundProperty); set => SetValue(TitleHeaderBackgroundProperty, value); }
        public bool IsShowTaskBar
        {
            get { return (bool)GetValue(IsShowTaskBarProperty); }
            set { SetValue(IsShowTaskBarProperty, value); }
        }
        #region Dimming
        public static readonly DependencyProperty DimmingProperty;
        public static readonly DependencyProperty DimmingColorProperty;
        public static readonly DependencyProperty DimmingOpacityProperty;
        public bool Dimming { get => (bool)GetValue(DimmingProperty); set => SetValue(DimmingProperty, value); }
        public Brush DimmingColor { get => (Brush)GetValue(DimmingColorProperty); set => SetValue(DimmingColorProperty, value); }
        public double DimmingOpacity { get => (double)GetValue(DimmingOpacityProperty); set => SetValue(DimmingOpacityProperty, value); }
        #endregion
        private MaximizeButton maximBtn;
        static DarkThemeWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DarkThemeWindow), new FrameworkPropertyMetadata(typeof(DarkThemeWindow)));
            CloseCommandProperty = DependencyProperty.Register(nameof(CloseCommand), typeof(ICommand), typeof(DarkThemeWindow), new PropertyMetadata(null));
            TitleProperty = DependencyProperty.Register(nameof(Title), typeof(object), typeof(DarkThemeWindow), new UIPropertyMetadata(null));
            TitleHeaderBackgroundProperty = DependencyProperty.Register(nameof(TitleHeaderBackground), typeof(Brush), typeof(DarkThemeWindow), new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#252525"))));
            DimmingProperty = DependencyProperty.Register(nameof(Dimming), typeof(bool), typeof(DarkThemeWindow), new PropertyMetadata(false, (e, a) =>
            {
                //Console.WriteLine ("");
            }));

            DimmingColorProperty = DependencyProperty.Register(nameof(DimmingColor), typeof(Brush), typeof(DarkThemeWindow), new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#141414"))));
            DimmingOpacityProperty = DependencyProperty.Register(nameof(DimmingOpacity), typeof(double), typeof(DarkThemeWindow), new PropertyMetadata(0.8));

            IsShowTaskBarProperty = DependencyProperty.Register("IsShowTaskBar", typeof(bool), typeof(DarkThemeWindow), new PropertyMetadata(true, (d, e) =>
            {
                var win = (DarkThemeWindow)d;
                win.MaxHeightSet();
            }));
        }
        public DarkThemeWindow()
        {
            MaxHeightSet();
            this.AllowsTransparency = true;
            this.WindowStyle = WindowStyle.None;
            this.StateChanged += (s, e) =>
            {
                maximBtn.IsMaximize = !maximBtn.IsMaximize;
            };
        }
        public override void OnApplyTemplate()
        {
            if (GetTemplateChild("PART_CloseButton") is CloseButton btn)
            {
                btn.Click += (s, e) => WindowClose();
            }

            if (GetTemplateChild("PART_MinButton") is MinimizeButton minbtn)
            {
                minbtn.Click += (s, e) => WindowState = WindowState.Minimized;
            }

            if (GetTemplateChild("PART_MaxButton") is MaximizeButton maxbtn)
            {
                maximBtn = maxbtn;
                maxbtn.Click += (s, e) =>
                {
                    this.WindowState = maxbtn.IsMaximize ? WindowState.Normal : WindowState.Maximized;
                };
            }
            if (GetTemplateChild("PART_DragBar") is DraggableBar bar)
            {
                bar.MouseDown += WindowDragMove;
            }
            maximBtn.IsMaximize = this.WindowState == WindowState.Maximized;
        }
        private void WindowClose()
        {
            if (CloseCommand == null)
            {
                Close();
            }
            else
            {
                CloseCommand.Execute(this);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }

        private void WindowDragMove(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                GetWindow(this).DragMove();
            }
        }
        private void MaxHeightSet()
        {
            this.MaxHeight = IsShowTaskBar ? SystemParameters.MaximizedPrimaryScreenHeight : Double.PositiveInfinity;
        }
    }
}
