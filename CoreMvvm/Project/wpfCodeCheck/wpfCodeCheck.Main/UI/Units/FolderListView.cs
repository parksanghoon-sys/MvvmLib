using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using wpfCodeCheck.Share.Enums;

namespace wpfCodeCheck.Main.UI.Units
{
    internal class FolderListView : ContentControl
    {
        public string DirectoryPath
        {
            get { return (string)GetValue(DirectoryPathProperty); }
            set { SetValue(DirectoryPathProperty, value); }
        }
        public static readonly DependencyProperty DirectoryPathProperty =
            DependencyProperty.Register("DirectoryPath", typeof(string), typeof(FolderListView), new PropertyMetadata("", OnCahngedTest));
        public EFolderListType FolderListType
        {
            get { return (EFolderListType)GetValue(FolderListTypeProperty); }
            set { SetValue(FolderListTypeProperty, value); }
        }
        public static readonly DependencyProperty FolderListTypeProperty =
            DependencyProperty.Register("FolderListType", typeof(EFolderListType), typeof(FolderListView), new PropertyMetadata(EFolderListType.INPUT));
        private static void OnCahngedTest(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Debug.WriteLine($"{e.NewValue}");
            var control = d as FolderListView;
            if (control != null)
            {
               control._textBlock.Tag = e.NewValue as string;
            }
        }

        static FolderListView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FolderListView), new FrameworkPropertyMetadata(typeof(FolderListView)));
        }
        TextBlock _textBlock;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _textBlock = this.GetTemplateChild("PART_TxtDirectoryPath") as TextBlock;
            this.Tag = FolderListType;
        }
    }
}
