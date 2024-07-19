using System.ComponentModel;
using System.Runtime.CompilerServices;
using wpfCodeCheck.Domain.Datas;

namespace wpfCodeCheck.Domain.Services
{
    public class BaseService : IBaseService
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private CodeDiffModel? _compareModel;
        public CodeDiffModel? CompareResult
        {
            get => _compareModel ?? throw new ArgumentNullException(nameof(CodeDiffModel), "CompareResults cannot be null");
            set
            {
                if (_compareModel != value)
                {
                    _compareModel = value;
                    OnPropertyChanged();
                }
            }
        }

        public void SetDirectoryCompareReuslt(CodeDiffModel compareResult)
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
