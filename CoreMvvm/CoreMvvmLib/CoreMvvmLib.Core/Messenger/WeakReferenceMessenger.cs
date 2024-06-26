namespace CoreMvvmLib.Core.Messenger
{
    public class WeakReferenceMessenger
    {
        #region Static Property
        public static WeakReferenceMessenger Default { get; } = new WeakReferenceMessenger();
        #endregion

        private Dictionary<Type, List<IMessangeHandler>> _receiverList = new();
        private Dictionary<Type, List<IAsyncMessageHandler>> _asyncReceiverList = new();

        #region Public Func
        public void Register<TReceiver, TMessage>(TReceiver receiver, Action<TReceiver, TMessage> callback) 
            where TReceiver : class
        {
            var handler = new MessageHandler<TReceiver, TMessage>(callback, receiver);
            if (_receiverList.ContainsKey(typeof(TReceiver)) == false)
            {
                _receiverList[typeof(TReceiver)] = new List<IMessangeHandler>();
            }
            _receiverList[typeof(TReceiver)].Add(handler);
        }
        public void UnRegister<TReceiver, TMessage>(TReceiver receiver, Action<TReceiver, TMessage> callback)
                where TReceiver : class
        {            
            if (_receiverList.ContainsKey(typeof(TReceiver)) == true)
            {
                var handler = new MessageHandler<TReceiver, TMessage>(callback, receiver);
                _receiverList[typeof(TReceiver)].Remove(handler);
            }            
        }
        public void Send<TMessage>(TMessage message)
        {
            List<IMessangeHandler> messangeHandlers = new List<IMessangeHandler>();
            foreach (var receiver in _receiverList)
            {
                foreach (var handler in receiver.Value)
                {
                    messangeHandlers.Add(handler);
                }
            }
            foreach (var handler in messangeHandlers)
            {
                handler.Callback(message);
            }
        }
        public void Send<TMessage, TReceiver>(TMessage message)
        {
            if (_receiverList.ContainsKey(typeof(TReceiver)) == false)
                throw new InvalidCastException("");
            var handlers = _receiverList[typeof(TReceiver)];
            foreach (var handler in handlers)
            {
                handler.Callback(message);
            }
            _receiverList[typeof(TReceiver)].RemoveAll((handler) =>
            {
                return handler.IsAlive();
            });
        } 
        #endregion
        #region Public Async Func
        public void AsyncRegister<TReceiver, TMessage>(TReceiver receiver, Func<TReceiver, TMessage, Task> callback) where TReceiver : class
        {
            var handler = new AsyncMessageHandler<TReceiver, TMessage>(callback, receiver);
            if (this._asyncReceiverList.ContainsKey(typeof(TReceiver)) == false)
            {
                this._asyncReceiverList[typeof(TReceiver)] = new List<IAsyncMessageHandler>();
            }
            this._asyncReceiverList[typeof(TReceiver)].Add(handler);
        }

        public async Task AsyncSend<TMessage>(TMessage message)
        {
            List<IAsyncMessageHandler> messageHandlers = new List<IAsyncMessageHandler>();

            foreach (var receiver in this._asyncReceiverList)
            {
                foreach (var handler in receiver.Value)
                {
                    if (handler.MessageType() == typeof(TMessage))
                    {
                        messageHandlers.Add(handler);
                    }
                }
            }
            foreach (var handler in messageHandlers)
                await handler.Callback(message);
        }

        public async Task AsyncSend<TMessage, TReceiver>(TMessage message) where TReceiver : Type
        {
            if (this._asyncReceiverList.ContainsKey(typeof(TReceiver)) == false)
            {
                throw new InvalidOperationException("There is no proper receiver type");
            }

            var handlers = this._asyncReceiverList[typeof(TReceiver)];

            foreach (var handler in handlers)
            {
                await handler.Callback(message);
            }

            this._asyncReceiverList[typeof(TReceiver)].RemoveAll((handler) =>
            {
                return handler.IsAlive();
            });
        }
        
        #endregion

    }
}
