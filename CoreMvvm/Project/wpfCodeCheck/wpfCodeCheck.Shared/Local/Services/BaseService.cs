using wpfCodeCheck.Shared.Local.Models;

namespace wpfCodeCheck.Shared.Local.Services
{
    public class BaseService : IBaseService
    {
        private CodeCompareModel _compareModel;
        public CodeCompareModel CompareResult => _compareModel;
    

        public void SetDirectoryCompareReuslt(CodeCompareModel compareResult)
        {
            _compareModel = compareResult;
        }
    }
}
