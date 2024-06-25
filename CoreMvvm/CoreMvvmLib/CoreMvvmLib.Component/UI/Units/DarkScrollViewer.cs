using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace CoreMvvmLib.Component.UI.Units
{
    public class DarkScrollViewer : ScrollViewer
    {
        static DarkScrollViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DarkScrollViewer), new FrameworkPropertyMetadata(typeof(DarkScrollViewer)));
        }
    }
}
