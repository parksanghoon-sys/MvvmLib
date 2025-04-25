using System.Windows.Media.Animation;
using System.Windows;
using CoreMvvmLib.Component.UI.Enums;

namespace CoreMvvmLib.Component.UI.Units
{

    public class ValueItem : DoubleAnimation
    {
        public static readonly DependencyProperty TargetNameProperty = DependencyProperty.Register("TargetName", typeof(string), typeof(ValueItem), new PropertyMetadata(null, OnTargetNameChanged));

        public static readonly DependencyProperty PropertyProperty = DependencyProperty.Register("Property", typeof(PropertyPath), typeof(ValueItem), new PropertyMetadata(null, OnPropertyChanged));

        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(EasingFunctionBaseMode), typeof(ValueItem), new PropertyMetadata(EasingFunctionBaseMode.CubicEaseIn, OnEasingModeChanged));

        public string TargetName
        {
            get
            {
                return (string)GetValue(TargetNameProperty);
            }
            set
            {
                SetValue(TargetNameProperty, value);
            }
        }

        public PropertyPath Property
        {
            get
            {
                return (PropertyPath)GetValue(PropertyProperty);
            }
            set
            {
                SetValue(PropertyProperty, value);
            }
        }

        public EasingFunctionBaseMode Mode
        {
            get
            {
                return (EasingFunctionBaseMode)GetValue(ModeProperty);
            }
            set
            {
                SetValue(ModeProperty, value);
            }
        }

        private static void OnTargetNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ValueItem element = (ValueItem)d;
            string name = (string)e.NewValue;
            Storyboard.SetTargetName(element, name);
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ValueItem element = (ValueItem)d;
            PropertyPath path = (PropertyPath)e.NewValue;
            Storyboard.SetTargetProperty(element, path);
        }

        private static void OnEasingModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ValueItem valueItem = (ValueItem)d;
            EasingFunctionBaseMode mode = (EasingFunctionBaseMode)e.NewValue;
            CubicEase cubicEase = valueItem.EasingFunction as CubicEase;
            if (cubicEase != null)
            {
                cubicEase.EasingMode = GetMode(mode);
            }
            else
            {
                valueItem.EasingFunction = GetEasingFunc(mode);
            }
        }

        private static IEasingFunction GetEasingFunc(EasingFunctionBaseMode mode)
        {
            EasingMode mode2 = GetMode(mode);
            EasingFunctionBase functonBase = GetFunctonBase(mode);
            functonBase.EasingMode = mode2;
            return functonBase;
        }

        private static EasingFunctionBase GetFunctonBase(EasingFunctionBaseMode mode)
        {
            return mode.ToString().Replace("EaseInOut", "").Replace("EaseIn", "")
                .Replace("EaseOut", "") switch
            {
                "Back" => new BackEase(),
                "Bounce" => new BounceEase(),
                "Circle" => new CircleEase(),
                "Cubic" => new CubicEase(),
                "Elastic" => new ElasticEase(),
                "Exponential" => new ExponentialEase(),
                "Power" => new PowerEase(),
                "Quadratic" => new QuadraticEase(),
                "Quartic" => new QuarticEase(),
                "Quintic" => new QuinticEase(),
                "Sine" => new SineEase(),
                _ => null,
            };
        }

        private static EasingMode GetMode(EasingFunctionBaseMode mode)
        {
            string text = mode.ToString();
            if (text.Contains("EaseInOut"))
            {
                return EasingMode.EaseInOut;
            }

            if (text.Contains("EaseIn"))
            {
                return EasingMode.EaseIn;
            }

            return EasingMode.EaseOut;
        }
    }
}
