using wpfCodeCheck.Shared.Local.Models;

namespace wpfCodeCheck.Shared.Local.Services
{
    public interface IBaseService
    {
        CodeCompareModel CompareResult { get; }        
        void SetDirectoryCompareReuslt(CodeCompareModel compareResult);
    }
}
