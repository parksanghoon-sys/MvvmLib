using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Input;

namespace wpfCodeCheck.Forms.Local.Behaviors
{
    public class WindowBehavior : Behavior<Window>
    {       
        // Using a DependencyProperty as the backing store for ClosedCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClosedCommandProperty =
            DependencyProperty.Register("ClosedCommand", typeof(ICommand), typeof(WindowBehavior), new PropertyMetadata(null));

        public static readonly DependencyProperty CommandParameterProperty =
           DependencyProperty.Register("CommandParameter", typeof(object), typeof(WindowBehavior), new PropertyMetadata(null));
        public ICommand ClosedCommand
        {
            get { return (ICommand)GetValue(ClosedCommandProperty); }
            set { SetValue(ClosedCommandProperty, value); }
        }
        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }        
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Closed += AssociatedObject_Closed;
        }
        private void AssociatedObject_Closed(object? sender, EventArgs e)
        {
            if (ClosedCommand != null && ClosedCommand.CanExecute(CommandParameter))
            {
                ClosedCommand.Execute(CommandParameter);
            }
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Closed -= AssociatedObject_Closed;
        }    
    }
}
