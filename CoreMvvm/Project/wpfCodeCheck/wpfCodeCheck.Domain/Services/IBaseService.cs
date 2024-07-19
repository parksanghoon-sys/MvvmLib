using System.ComponentModel;
using wpfCodeCheck.Domain.Datas;

namespace wpfCodeCheck.Domain.Services
{
    public interface IBaseService : INotifyPropertyChanged
    {
        CodeCompareResultModel CompareResult { get; }
        void SetDirectoryCompareReuslt(CodeCompareResultModel compareResult);
    }
}
