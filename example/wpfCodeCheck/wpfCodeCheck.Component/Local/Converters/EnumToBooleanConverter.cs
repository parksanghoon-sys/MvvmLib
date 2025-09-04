﻿using CoreMvvmLib.WPF;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace wpfCodeCheck.Component.Local.Converters
{
    public class EnumToBooleanConverter : ConverterMarkupExtension<EnumToBooleanConverter>
    {        
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Enum)value).HasFlag((Enum)parameter);
        }        

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.Equals(true) ? parameter : Binding.DoNothing;
        }
    }
}
