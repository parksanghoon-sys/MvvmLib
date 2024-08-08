using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using CoreMvvmLib.Component.UI.Units;
using wpfCodeCheck.Domain.Helpers;

namespace wpfCodeCheck.Component.UI.Units
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
        Button button;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            button = this.GetTemplateChild("PART_btnPath") as Button;

            button.Click += (s, e) =>
            {
                BrowseForFolderDialog dlg = new BrowseForFolderDialog();
                dlg.Title = "Select a folder and click OK!";
                dlg.InitialExpandedFolder = DirectoryHelper.GetMyDocumentsDirectory();
                dlg.OKButtonText = "OK!";
                if (true == dlg.ShowDialog())
                {
                    this.Text = dlg.SelectedFolder;
                    //if (string.Equals(type, "Input"))
                    //    InputDirectoryPath = dlg.SelectedFolder;
                    //else
                    //    OutputDirectoryPath = dlg?.SelectedFolder;
                }
            };
            this.PreviewDragOver += (s, e) => { e.Handled = true; };

            this.Drop += textBlock_Drop;
            

        }
        private void textBlock_Drop(object sender, DragEventArgs e)
        {

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                // 만약 파일을 드레그 했을 경우 파일 명 제외 폴더 경로 추출
                //var filePath  = string.Join('\\',files.First().Split('\\').Where(p => p.Contains('.') == false).ToList());
                // 파일 경로들을 개행 문자로 결합하여 문자열 생성
                string filePaths = string.Join("\n", files);

                // TextBox에 파일 경로 문자열 할당
                this.Text = filePaths;
            }
        }
    }
}
