using CoreMvvmLib.Component.UI.Units;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace wpfCodeCheck.Forms.Local.Models
{
    public class NavigationModeal : INotifyPropertyChanged
    {
        private bool _isEnable;

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public IconType IconType { get; set; }
        public string Name { get; set; }

        public bool IsEnable
        {
            get { return _isEnable; }
            set { _isEnable = value; OnPropertyChanged(); }
        }
        
        public NavigationModeal(IconType type, string name, bool isEnable)
        {
            IconType = type;
            Name = name;
            IsEnable = isEnable;
        }
    }
}
