using CoreMvvmLib.WPF;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace wpfCodeCheck.SDDExport.Local.Converters;

public class DepthConverter : ConverterMarkupExtension<DepthConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        int depth = (int)value;
        Thickness margin = new Thickness(depth* 20, 0,0,0);
        return margin;
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }    
    
}

