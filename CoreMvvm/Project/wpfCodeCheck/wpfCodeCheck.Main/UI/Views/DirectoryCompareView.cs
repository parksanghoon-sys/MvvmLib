using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace wpfCodeCheck.Main.UI.Views
{    
    public class DirectoryCompareView : ContentControl
    {
        static DirectoryCompareView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DirectoryCompareView), new FrameworkPropertyMetadata(typeof(DirectoryCompareView)));
        }
    }
}
