using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wpfCodeCheck.Shared.Local.Models;

namespace wpfCodeCheck.Shared.Local.Services
{
    public class BaseService : IBaseService
    {
        private IList<CodeInfo> _inputCodeInfos;
        private IList<CodeInfo> _outputCoideInfos;
        public IList<CodeInfo> InputCodeInfos  { get { return _inputCodeInfos; } }
        public IList<CodeInfo> OutputCodeInfos  { get { return _outputCoideInfos; } }


        public void SetCodeInfos(IList<CodeInfo> inputCodeInfos, IList<CodeInfo> outputCodeInfos)
        {
            _inputCodeInfos = inputCodeInfos;
            _outputCoideInfos = outputCodeInfos;
        }
    }
}
