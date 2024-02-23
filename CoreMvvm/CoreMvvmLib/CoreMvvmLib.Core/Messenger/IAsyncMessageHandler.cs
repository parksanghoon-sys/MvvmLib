namespace CoreMvvmLib.Core.Messenger
{
    public interface IAsyncMessageHandler
    {
        public Type MessageType();
        public Type ReceiverType();

        public Task Callback(object message);

        public bool IsAlive();
    }
}
