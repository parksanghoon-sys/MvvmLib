using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpfCodeCheck.Shared.Local.Helper.Settings
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class SettingAttribute : Attribute
    {
        private object _defaultValue = null;
        public SettingAttribute(object defaultValeu)
        {
            _defaultValue = defaultValeu;
        }
        public object DefaultValue
        {
            get { return _defaultValue; }
        }
    }
}
