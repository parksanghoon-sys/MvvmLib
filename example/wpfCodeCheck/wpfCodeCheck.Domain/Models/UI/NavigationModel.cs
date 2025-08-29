using wpfCodeCheck.Domain.Models.Base;
using wpfCodeCheck.Domain.Enums;

namespace wpfCodeCheck.Domain.Models.UI
{
    public class NavigationModel : BaseModel
    {
        private bool _isEnable;

        public IconType IconType { get; set; }
        public string Name { get; set; } = string.Empty;

        public bool IsEnable
        {
            get => _isEnable;
            set => SetProperty(ref _isEnable, value);
        }

        public NavigationModel(IconType type, string name, bool isEnable)
        {
            IconType = type;
            Name = name;
            IsEnable = isEnable;
        }
    }
}