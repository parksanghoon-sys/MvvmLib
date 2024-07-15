using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace CoreMvvmLib.WPF.MarkupExtensions
{
    /// <summary>
    /// IValueConverter 상속받는 Converter을 Markup에 등록해주는 클래스
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="System.Windows.Markup.MarkupExtension" />
    /// <seealso cref="System.Windows.Data.IValueConverter" />
    public abstract class ConverterMarkupExtension<T> : MarkupExtension, IValueConverter
        where T : class, new()
    {

        /// <summary>
        /// IValueConvert를 상속 받는 class
        /// </summary>
        private static Lazy<T> _converter = new Lazy<T>(() => new T());
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter.Value;
        }
        /// <summary>
        /// 값을 변환합니다.
        /// </summary>
        /// <param name="value">바인딩 소스에서 생성한 값입니다.</param>
        /// <param name="targetType">바인딩 대상 속성의 형식입니다.</param>
        /// <param name="parameter">사용할 변환기 매개 변수입니다.</param>
        /// <param name="culture">변환기에서 사용할 문화권입니다.</param>
        /// <returns>
        /// 변환된 값입니다. 메서드가 null을 반환하면 올바른 null 값이 사용됩니다.
        /// </returns>
        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);
        /// <summary>
        /// Convert 실패 값을 변환합니다.
        /// </summary>
        /// <param name="value">바인딩 대상에서 생성한 값입니다.</param>
        /// <param name="targetType">변환할 대상 형식입니다.</param>
        /// <param name="parameter">사용할 변환기 매개 변수입니다.</param>
        /// <param name="culture">변환기에서 사용할 문화권입니다.</param>
        /// <returns>
        /// 변환된 값입니다. 메서드가 null을 반환하면 올바른 null 값이 사용됩니다.
        /// </returns>
        public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
    }
}
