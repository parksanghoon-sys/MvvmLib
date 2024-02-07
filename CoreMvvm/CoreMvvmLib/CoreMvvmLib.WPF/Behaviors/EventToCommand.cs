using Microsoft.Xaml.Behaviors;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace CoreMvvmLib.WPF.Behaviors
{
    public class EventToCommand : Behavior<FrameworkElement>
    {
        Delegate eventHandler;
        
        public static readonly DependencyProperty EventNameProperty =
            DependencyProperty.Register("EventName", typeof(string), typeof(EventToCommand), new PropertyMetadata("",OnEvnetNameChanged));
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(EventToCommand), new PropertyMetadata(null));        
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(EventToCommand), new PropertyMetadata(null));
        public static readonly DependencyProperty InpuConverterProperty =
            DependencyProperty.Register("InpuConverter", typeof(IValueConverter), typeof(EventToCommand), new PropertyMetadata(null));

        public string EventName
        {
            get { return (string)GetValue(EventNameProperty); }
            set { SetValue(EventNameProperty, value); }
        }
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public IValueConverter InpuConverter
        {
            get { return (IValueConverter)GetValue(InpuConverterProperty); }
            set { SetValue(InpuConverterProperty, value); }
        }
        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            RegisterEvent(EventName);
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            DeregisterEvnet(EventName);
        }
        private static void OnEvnetNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as EventToCommand;
            if (behavior.AssociatedObject == null)
                return;
            string oldEventName = e.OldValue as string;
            string newEventName = e.NewValue as string;

            behavior.DeregisterEvnet(oldEventName);
            behavior.RegisterEvent(newEventName);
        }
        private void RegisterEvent(string eventName)
        {
            if (string.IsNullOrEmpty(eventName))
                return;

            EventInfo eventInfo = AssociatedObject.GetType().GetRuntimeEvent(eventName);
            if (eventInfo == null)
                return;

            MethodInfo methodInfo = typeof(EventToCommand).GetTypeInfo().GetDeclaredMethod("OnEvent");
            eventHandler = methodInfo.CreateDelegate(eventInfo.EventHandlerType, this);
            eventInfo.AddEventHandler(AssociatedObject, eventHandler);

        }
        private void DeregisterEvnet(string eventName)
        {
            if (string.IsNullOrWhiteSpace(eventName))
            {
                return;
            }

            if (eventHandler == null)
            {
                return;
            }

            EventInfo eventInfo = AssociatedObject.GetType().GetRuntimeEvent(eventName);
            if (eventInfo == null)
            {
                throw new ArgumentException("Invalid event name : " + eventName);
            }
            eventInfo.RemoveEventHandler(AssociatedObject, eventHandler);
            eventHandler = null;
        }
        public void OnEvent(object sender, object eventArgs)
        {
            if (Command == null)
                return;
            object resolveParameter = null;

            if (CommandParameter != null)
                resolveParameter = CommandParameter;
            else if (InpuConverter != null)
                resolveParameter = InpuConverter.Convert(eventArgs, typeof(object), null, null);
            else
                resolveParameter = eventArgs;

            if(Command.CanExecute(resolveParameter))
            {
                Command.Execute(resolveParameter);
            }

        }
      

        
    }
}
