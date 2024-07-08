using CoreMvvmLib.Component.UI.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace wpfCodeCheck.Shared.UI.Units
{
    public class BaseButton : Button
    {
        public IconType IconType
        {
            get { return (IconType)GetValue(IconTypeProperty); }
            set { SetValue(IconTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconTypeProperty =
            DependencyProperty.Register("IconType", typeof(IconType), typeof(BaseButton), new PropertyMetadata(null));


        static BaseButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BaseButton), new FrameworkPropertyMetadata(typeof(BaseButton)));
        }
    }
}
