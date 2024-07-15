using CoreMvvmLib.Component.UI.Units;

namespace wpfCodeCheck.Forms.Local.Models
{
    public class NavigationModeal
    {
        public IconType IconType { get; set; }
        public string Name { get; set; }
        public bool IsEnable { get; set; } = true;

        public NavigationModeal(IconType type, string name, bool isEnable)
        {
            IconType = type;
            Name = name;
            IsEnable = isEnable;
        }
    }
}
