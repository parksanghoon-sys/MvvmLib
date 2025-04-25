using CoreMvvmLib.Component.UI.Views;
using System.Windows;
using System.Windows.Controls;
using wpfCodeCheck.Forms.UI.Units;

namespace wpfCodeCheck.Forms.UI.Views
{
    public class MainWindowView : DarkThemeWindow
    {
        static MainWindowView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MainWindowView), new FrameworkPropertyMetadata(typeof(MainWindowView)));
        }
        private TabContent content;
        private SlideMenuBar slideMenu;
        private ContentControl content1;

        public override void OnApplyTemplate()
        {            
            slideMenu = GetTemplateChild("PART_ITEM") as SlideMenuBar;
            content = GetTemplateChild("PART_CONTENT") as TabContent;
            content1 = GetTemplateChild("content") as ContentControl;
            if (slideMenu != null)
            {
                slideMenu.SelectedItemChanged += ItemSelectionChanged;
            }
            base.OnApplyTemplate();
        }
        private void ItemSelectionChanged(object sender, RoutedEventArgs e)
        {
            //content.ModeScroll();
        }
    }

}
