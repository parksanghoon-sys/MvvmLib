using System.ComponentModel;
using System.Runtime.CompilerServices;
using wpfCodeCheck.Domain.Datas;

namespace wpfCodeCheck.Domain.Services
{
    public class CustomBaseService<T> : IBaseService<T>
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private CodeDiffReulstModel<T>? _compareModel;    

        public CodeDiffReulstModel<T> CompareResult
        {
            get => _compareModel ?? throw new ArgumentNullException(nameof(T), "CompareResults cannot be null");
            private set => _compareModel = value;
        }

        public void SetDirectoryCompareReuslt(CodeDiffReulstModel<T> compareResult)
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
