using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using wpfCodeCheck.Domain.Models;

namespace wpfCodeCheck.SDDExport.UI.Units
{
    public class FileTreeItem : TreeViewItem
    {


        public ICommand SelectionCommand
        {
            get { return (ICommand)GetValue(SelectionCommandProperty); }
            set { SetValue(SelectionCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectionCommandProperty =
            DependencyProperty.Register("SelectionCommand", typeof(ICommand), typeof(FileTreeItem), new PropertyMetadata(null));


        static FileTreeItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FileTreeItem), new FrameworkPropertyMetadata(typeof(FileTreeItem)));
        }
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new FileTreeItem();
        }
        public FileTreeItem()
        {
            MouseLeftButtonUp += FileTreeItem_MouseLeftButtonUp;
        }

        private void FileTreeItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            if (DataContext is FileTreeModel item)
            {
                SelectionCommand?.Execute(item);
            }
        }
    }

}
