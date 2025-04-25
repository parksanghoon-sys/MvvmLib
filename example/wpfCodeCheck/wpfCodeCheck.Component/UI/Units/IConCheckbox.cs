using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace wpfCodeCheck.Component.UI.Units
{
    public class IConCheckbox : CheckBox
    {
        static IConCheckbox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IConCheckbox), new FrameworkPropertyMetadata(typeof(IConCheckbox)));
        }
    }
}
