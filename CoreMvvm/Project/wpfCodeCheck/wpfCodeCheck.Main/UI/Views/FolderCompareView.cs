using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace wpfCodeCheck.Main.UI.Views
{
    public class FolderCompareView : ContentControl
    {
        static FolderCompareView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FolderCompareView), new FrameworkPropertyMetadata(typeof(FolderCompareView)));
        }
    }
}
