using System.ComponentModel;
using wpfCodeCheck.Shared.Local.Models;

namespace wpfCodeCheck.Shared.Local.Services
{
    public interface IBaseService : INotifyPropertyChanged
    {
        CodeCompareModel CompareResult { get; }        
        void SetDirectoryCompareReuslt(CodeCompareModel compareResult);
    }
}
