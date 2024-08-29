using System.ComponentModel;
using wpfCodeCheck.Domain.Datas;

namespace wpfCodeCheck.Domain.Services
{
    public interface IBaseService<T> : INotifyPropertyChanged
    {
        CodeDiffReulstModel<T> CompareResult { get; }
        void SetDirectoryCompareReuslt(CodeDiffReulstModel<T> compareResult);
    }
}
