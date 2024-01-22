using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreMvvmLib.Core.IOC
{
    public interface IServiceContainer
    {
        public TInterface GetService<TInterface>() where TInterface : class;
        public object GetService(Type serviceType);
        public Type KeyType(string key);
    }
}
