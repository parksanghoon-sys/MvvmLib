using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;

namespace wpfCodeCheck.Main.UI.Units
{
    public class LocatorTextBox : TextBox
    {
        public ICommand FileOpenCommand
        {
            get { return (ICommand)GetValue(FileOpenCommandProperty); }
            set { SetValue(FileOpenCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FileOpenCommandProperty =
            DependencyProperty.Register("FileOpenCommand", typeof(ICommand), typeof(LocatorTextBox), new PropertyMetadata(null));

        public string FileOpenCommandParameter
        {
            get { return (string)GetValue(FileOpenCommandParameterProperty); }
            set { SetValue(FileOpenCommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FileOpenCommandParameterProperty =
            DependencyProperty.Register("FileOpenCommandParameter", typeof(string), typeof(LocatorTextBox), new PropertyMetadata(null));
        static LocatorTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LocatorTextBox), new FrameworkPropertyMetadata(typeof(LocatorTextBox)));
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.PreviewDragOver += (s, e) => { e.Handled = true; };

            this.Drop += textBlock_Drop;

        }
        private void textBlock_Drop(object sender, DragEventArgs e)
        {

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // 파일 경로들을 개행 문자로 결합하여 문자열 생성
                string filePaths = string.Join("\n", files);

                // TextBox에 파일 경로 문자열 할당
                this.Text = filePaths;
            }
        }
    }
}
