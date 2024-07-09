using wpfCodeCheck.Shared.Local.Models;

namespace wpfCodeCheck.Shared.Local.Services
{
    public interface IBaseService
    {
        void SetCodeInfos(IList<CodeCompareModel> inputCoideInfos, IList<CodeCompareModel> outputCodeInfos);
    }
}
