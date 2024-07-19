using System.ComponentModel;
using wpfCodeCheck.Domain.Datas;

namespace wpfCodeCheck.Domain.Services
{
    public interface IBaseService : INotifyPropertyChanged
    {
        CodeDiffModel CompareResult { get; }
        void SetDirectoryCompareReuslt(CodeDiffModel compareResult);
    }
}
