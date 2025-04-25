using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace wpfCodeCheck.SDDExport.UI.Units
{
    public class FileTreeView : TreeView
    {
        static FileTreeView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FileTreeView), new FrameworkPropertyMetadata(typeof(FileTreeView)));
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new FileTreeItem();
        }
    }

}
