using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wpfCodeCheck.Domain.Datas;

namespace wpfCodeCheck.Domain.Services
{
    public interface ICompare
    {        
        public Task<List<CompareEntity>> CompareModelCollections(IList<FileCompareModel> inputItems, IList<FileCompareModel> outputItems);
        public Task<bool> CreateCompareOutputInfo();
    }
}
