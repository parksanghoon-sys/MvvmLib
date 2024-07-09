using wpfCodeCheck.Shared.Local.Models;

namespace wpfCodeCheck.Shared.Local.Services
{
    public interface IBaseService
    {
        IList<CodeCompareModel> InputCodeCompareInfos { get; }
        IList<CodeCompareModel> OutputCodeCompareInfos { get; }
        void SetCodeInfos(IList<CodeCompareModel> inputCoideInfos, IList<CodeCompareModel> outputCodeInfos);
    }
}
