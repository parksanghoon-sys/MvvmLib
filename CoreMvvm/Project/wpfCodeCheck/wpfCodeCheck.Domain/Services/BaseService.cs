using System.ComponentModel;
using System.Runtime.CompilerServices;
using wpfCodeCheck.Domain.Datas;

namespace wpfCodeCheck.Domain.Services
{
    public class BaseService : IBaseService
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private CodeCompareResultModel _compareModel;
        public CodeCompareResultModel CompareResult
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

        public void SetDirectoryCompareReuslt(CodeCompareResultModel compareResult)
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
