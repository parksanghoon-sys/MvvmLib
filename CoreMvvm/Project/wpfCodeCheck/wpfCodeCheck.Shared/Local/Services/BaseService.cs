using wpfCodeCheck.Shared.Local.Models;

namespace wpfCodeCheck.Shared.Local.Services
{
    public class BaseService : IBaseService
    {
        private IList<CodeCompareModel> _inputCodeInfos;
        private IList<CodeCompareModel> _outputCoideInfos;
        public IList<CodeCompareModel> InputCodeInfos  { get { return _inputCodeInfos; } }
        public IList<CodeCompareModel> OutputCodeInfos  { get { return _outputCoideInfos; } }


        public void SetCodeInfos(IList<CodeCompareModel> inputCodeInfos, IList<CodeCompareModel> outputCodeInfos)
        {
            _inputCodeInfos = inputCodeInfos;
            _outputCoideInfos = outputCodeInfos;
        }
    }
}
