namespace CoreMvvmLib.Core.Messenger
{
    internal class MessageHandler<TReceiver, TMessage> : IMessangeHandler where TReceiver : class
    {
        private readonly Action<TReceiver, TMessage> _handler;
        private WeakReference<TReceiver> _receiver;

        public MessageHandler(Action<TReceiver, TMessage> handler,TReceiver receiver)
        {
            _handler = handler;
            _receiver = new WeakReference<TReceiver>(receiver);
        }
        public void Callback(object message)
        {
            var typeStr = message.GetType().Name;
            if(_handler is not null && MesssageType() == message.GetType())
            {
                TReceiver receiver;
                if(_receiver.TryGetTarget(out receiver))
                {
                   _handler(receiver, (TMessage)message);
                }
            }
        }

        public bool IsAlive()
        {
            TReceiver receiver = null;
            _receiver.TryGetTarget(out receiver);
            return receiver != null;
        }

        public Type MesssageType()
        {
            return typeof(TMessage);
        }

        public Type ReceiverType()
        {
            return typeof(TReceiver);
        }
    }
}
