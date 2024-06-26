﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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


        static LocatorTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LocatorTextBox), new FrameworkPropertyMetadata(typeof(LocatorTextBox)));
        }
    }
}
