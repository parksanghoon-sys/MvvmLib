namespace CoreMvvmLib.Messenger
{
    public interface IMessangeHandler
    {
        public Type MesssageType();
        public Type ReceiverType();
        public void Callback(object message);
        public bool IsAlive();
    }
}
