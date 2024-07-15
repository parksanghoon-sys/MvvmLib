using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace wpfCodeCheck.Forms.Local.Behaviors
{
    internal class WindowBehavior : Behavior<Window>
    {
        public WindowBehavior()
        {
            
        }
        protected override void OnAttached()
        {
            base.OnAttached(); 
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
        }
    }
}
