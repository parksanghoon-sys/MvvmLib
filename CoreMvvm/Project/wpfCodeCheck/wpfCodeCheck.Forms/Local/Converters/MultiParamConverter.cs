
using CoreMvvmLib.WPF;
using System.Globalization;

namespace wpfCodeCheck.Forms.Local.Converters
{
    public class MultiParamConverter : MultiConverterMarkupExtension<MultiParamConverter>
    {        

        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Clone();
        }

        public override object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
