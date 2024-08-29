using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace wpfCodeCheck.Component.UI.Units
{
    public class BakcgroundRadioButton : RadioButton
    {
        static BakcgroundRadioButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BakcgroundRadioButton), new FrameworkPropertyMetadata(typeof(BakcgroundRadioButton)));
        }
    }
}
