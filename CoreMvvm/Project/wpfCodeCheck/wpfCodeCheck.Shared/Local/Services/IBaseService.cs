using System.ComponentModel;
using wpfCodeCheck.Shared.Local.Models;

namespace wpfCodeCheck.Shared.Local.Services
{
    public interface IBaseService : INotifyPropertyChanged
    {
        CodeCompareResultModel CompareResult { get; }        
        void SetDirectoryCompareReuslt(CodeCompareResultModel compareResult);
    }
}
