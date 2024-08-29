using System.ComponentModel;
using wpfCodeCheck.Domain.Datas;

namespace wpfCodeCheck.Domain.Services
{
    public interface IBaseService<T> : INotifyPropertyChanged
    {
        DiffReulstModel<T> CompareResult { get; }
        void SetDirectoryCompareReuslt(DiffReulstModel<T> compareResult);
    }
}
