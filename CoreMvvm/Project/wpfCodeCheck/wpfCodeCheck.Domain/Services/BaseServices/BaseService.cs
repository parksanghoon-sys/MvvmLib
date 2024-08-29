using System.ComponentModel;
using System.Runtime.CompilerServices;
using wpfCodeCheck.Domain.Datas;

namespace wpfCodeCheck.Domain.Services
{
    public class BaseService<T> : IBaseService<T>
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private DiffReulstModel<T>? _compareModel;    

        public DiffReulstModel<T> CompareResult
        {
            get => _compareModel ?? throw new ArgumentNullException(nameof(T), "CompareResults cannot be null");
            private set => _compareModel = value;
        }

        public void SetDirectoryCompareReuslt(DiffReulstModel<T> compareResult)
        {
            CompareResult = compareResult;
            OnPropertyChanged(nameof(CompareResult));
        }
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
     
    }
}
