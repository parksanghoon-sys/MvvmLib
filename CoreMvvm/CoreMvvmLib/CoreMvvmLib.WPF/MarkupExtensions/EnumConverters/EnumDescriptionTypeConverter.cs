using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoreMvvmLib.WPF.MarkupExtensions.EnumConverters
{
    /// <summary>
    /// Markup을 통한 Enum 값 매핑 시 [Discription("")] 으로 매핑을 위한 Converter
    /// 사용시 Enum 에 Attribute 지정 후 EnumBindingExtension 과 같이 사용시 설명으로 Return
    /// [TypeConverter(typeof(EnumDescriptionTypeConverter))]    
    /// </summary>
    public class EnumDescriptionTypeConverter : EnumConverter
    {
        public EnumDescriptionTypeConverter(Type type) : base(type)
        {

        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value != null)
                {
                    FieldInfo fi = value.GetType().GetField(value.ToString());
                    if (fi != null)
                    {
                        var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                        return attributes[0].Description;
                    }
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
