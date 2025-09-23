namespace CoreMvvmLib.Messenger
{
    internal class AsyncMessageHandler<TReceiver, TMessage> : IAsyncMessageHandler where TReceiver : class
    {
        private readonly Func<TReceiver, TMessage, Task> _handler;
        private WeakReference<TReceiver> _receiver;

        public AsyncMessageHandler(Func<TReceiver, TMessage, Task> func, TReceiver receiver)
        {
            _handler = func;
            _receiver = new WeakReference<TReceiver>(receiver);
        }
        public async Task Callback(object message)
        {
            if(_handler is not null)
            {
                TReceiver receiver;
                if(_receiver.TryGetTarget(out receiver))
                {
                    await _handler(receiver, (TMessage)message);
                }
            }
        }

        public bool IsAlive()
        {
            TReceiver receiver = null;
            _receiver?.TryGetTarget(out receiver);
            return _receiver != null;
        }

        public Type MessageType()
        {
            return typeof(TMessage);
        }

        public Type ReceiverType()
        {
            return typeof(TReceiver);
        }
    }
}
