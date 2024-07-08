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
        private IList<CodeInfo> _inputCoideInfos;
        private IList<CodeInfo> _outputCoideInfos;

        public void SetCodeInfos(IList<CodeInfo> inputCoideInfos, IList<CodeInfo> outputCodeInfos)
        {
            _inputCoideInfos = inputCoideInfos;
            _outputCoideInfos = outputCodeInfos;
        }
    }
}
