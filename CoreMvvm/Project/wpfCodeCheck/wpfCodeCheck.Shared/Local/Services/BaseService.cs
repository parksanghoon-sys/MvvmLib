using System.ComponentModel;
using System.Runtime.CompilerServices;
using wpfCodeCheck.Shared.Local.Models;

namespace wpfCodeCheck.Shared.Local.Services
{
    public class BaseService : IBaseService
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private CodeCompareModel _compareModel;
        public CodeCompareModel CompareResult
        {
            get => _compareModel;
            set
            {
                if (_compareModel != value)
                {
                    _compareModel = value;
                    OnPropertyChanged();
                }
            }
        }

        public void SetDirectoryCompareReuslt(CodeCompareModel compareResult)
        {
            _compareModel = compareResult;
        }
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
