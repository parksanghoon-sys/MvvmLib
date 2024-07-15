using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using wpfCodeCheck.Main.UI.Units;

namespace wpfCodeCheck.Main.UI.Views
{    
    public class DirectoryCompareView : ContentControl
    {
        static DirectoryCompareView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DirectoryCompareView), new FrameworkPropertyMetadata(typeof(DirectoryCompareView)));
        }
        LocatorTextBox inputText;
        LocatorTextBox outputText;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            inputText = GetTemplateChild("PART_InputTextBlock") as LocatorTextBox;
            outputText = GetTemplateChild("PART_OutputTextBlock") as LocatorTextBox;

            if (inputText != null && outputText != null)
            {
                inputText.PreviewDragOver += (s, e) => { e.Handled = true; };
                outputText.PreviewDragOver += (s, e) => { e.Handled = true; };

                inputText.Drop += textBlock_Drop;
                outputText.Drop += textBlock_Drop;
            }
        }

        private void textBlock_Drop(object sender, DragEventArgs e)
        {
            var text = sender as LocatorTextBox;

            if(e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // 파일 경로들을 개행 문자로 결합하여 문자열 생성
                string filePaths = string.Join("\n", files);

                // TextBox에 파일 경로 문자열 할당
                text.Text = filePaths;
            }
        }
    }
}
