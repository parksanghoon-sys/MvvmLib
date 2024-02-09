using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreMvvmLib.Core.Messenger
{
    public interface IMessangeHandler
    {
        public Type MesssageType();
        public Type ReceiverType();
        public void Callback(object message);
        public bool IsAlive();
    }
}
