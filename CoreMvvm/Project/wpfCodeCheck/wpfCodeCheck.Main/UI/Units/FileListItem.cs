﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace wpfCodeCheck.Main.UI.Units
{
    public class FileListItem : ListBoxItem
    {
        static FileListItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FileListItem), new FrameworkPropertyMetadata(typeof(FileListItem)));
        }
    }
}