using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace wpfCodeCheck.Forms.UI.Units
{
    public class SlideMenuBar : ContentControl
    {
        public Brush SelectedItemColor
        {
            get { return (Brush)GetValue(SelectedItemColorProperty); }
            set { SetValue(SelectedItemColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedItemColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemColorProperty =
            DependencyProperty.Register("SelectedItemColor", typeof(Brush), typeof(SlideMenuBar), new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6dbddd"))));

        public ICommand SelectedItem
        {
            get { return (ICommand)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(ICommand), typeof(SlideMenuBar), new PropertyMetadata(null));
        public IEnumerable ItemSource
        {
            get { return (IEnumerable)GetValue(ItemSourceProperty); }
            set { SetValue(ItemSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemSourceProperty =
            DependencyProperty.Register("ItemSource", typeof(IEnumerable), typeof(SlideMenuBar), new PropertyMetadata(null));
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(SlideMenuBar), new PropertyMetadata(false));
        static SlideMenuBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SlideMenuBar), new FrameworkPropertyMetadata(typeof(SlideMenuBar)));
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_ItemBoxSetting();
        }

        private void PART_ItemBoxSetting()
        {
            var itembox = GetTemplateChild("PART_ItemsBox") as ListBox;

            if(itembox != null)
            {
                itembox.SelectionChanged += (s, e) =>
                {
                    SelectedItem?.Execute(itembox.SelectedItem);
                };
                itembox.SelectedIndex = 0;
            }
        }
    }
}
