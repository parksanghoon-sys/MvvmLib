using System.Windows.Markup;

namespace CoreMvvmLib.WPF.Extensions
{
    /// <summary>
    /// Enum 을 Markup에 사용하기 위한 Class
    /// </summary>
    /// <seealso cref="System.Windows.Markup.MarkupExtension" />
    public class EnumBindingExtension : MarkupExtension
    {
        /// <summary>
        /// Enum Type
        /// </summary>
        public Type EnumType { get; private set; }
        /// <summary>
        /// 클래스 생성자
        /// </summary>
        /// <param name="type"></param>
        /// <exception cref="ArgumentException"></exception>
        public EnumBindingExtension(Type type)
        {
            if (type == null || !type.IsEnum)
                throw new ArgumentException("Not Enum");
            EnumType = type;
        }
        /// <summary>
        /// Enum 값을 반환하기 위한 함수
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns>Enum의 값</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Enum.GetValues(EnumType);
        }
    }
}
