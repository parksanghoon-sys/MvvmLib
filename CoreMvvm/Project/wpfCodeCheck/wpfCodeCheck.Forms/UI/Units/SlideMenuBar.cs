using Microsoft.Xaml.Behaviors.Core;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace wpfCodeCheck.Forms.UI.Units
{
    public class SlideMenuBar : ContentControl
    {        
        // Using a DependencyProperty as the backing store for SelectedIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(SlideMenuBar), new PropertyMetadata(0, OnChangedSelectedIndex));

        // Using a DependencyProperty as the backing store for SelectedItemColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemColorProperty =
            DependencyProperty.Register("SelectedItemColor", typeof(Brush), typeof(SlideMenuBar), new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6dbddd"))));

        // Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(ICommand), typeof(SlideMenuBar), new PropertyMetadata(null));        
        // Using a DependencyProperty as the backing store for ItemSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemSourceProperty =
            DependencyProperty.Register("ItemSource", typeof(IEnumerable), typeof(SlideMenuBar), new PropertyMetadata(null));        

        // Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(SlideMenuBar), new PropertyMetadata(false));
        public static readonly RoutedEvent SelectedItemChangedEvent =
                EventManager.RegisterRoutedEvent("SelectedItemChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SlideMenuBar));

        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }
        public Brush SelectedItemColor
        {
            get { return (Brush)GetValue(SelectedItemColorProperty); }
            set { SetValue(SelectedItemColorProperty, value); }
        }
        public ICommand SelectedItem
        {
            get { return (ICommand)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }
        public IEnumerable ItemSource
        {
            get { return (IEnumerable)GetValue(ItemSourceProperty); }
            set { SetValue(ItemSourceProperty, value); }
        }
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        public event RoutedEventHandler SelectedItemChanged
        {
            add { AddHandler(SelectedItemChangedEvent, value); }
            remove { RemoveHandler(SelectedItemChangedEvent, value); }
        }
     
        static SlideMenuBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SlideMenuBar), new FrameworkPropertyMetadata(typeof(SlideMenuBar)));
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_ItemBoxSetting();
        }
        ListBox? itembox;
        private void PART_ItemBoxSetting()
        {
            itembox = GetTemplateChild("PART_ItemsBox") as ListBox;

            if(itembox != null)
            {
                itembox.SelectionChanged += (s, e) =>
                {
                    SelectedItem?.Execute(itembox.SelectedItem);
                };
                itembox.SelectedIndex = 0;
            }
        }
        private void OnSelectedItemChanged()
        {
            RoutedEventArgs args = new RoutedEventArgs(SelectedItemChangedEvent);
            RaiseEvent(args);
        }
        private static void OnChangedSelectedIndex(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue)
            {
                var slideBar = d as SlideMenuBar;
                if (slideBar is not null)
                {
                    slideBar.itembox.SelectedIndex = (int)e.NewValue;
                }
            }
        }
    }
}
