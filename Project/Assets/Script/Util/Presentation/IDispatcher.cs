namespace Backend.Util.Presentation
{
    public interface IDispatcher
    {
        public void Subscribe(ISubscriber subscriber);

        public void Unsubscribe(ISubscriber subscriber);

        public void DispatchTo<TSubscriber, TMessage>(int index, TMessage message) where TSubscriber : ISubscriber;

        public void DispatchToAll<T>(T message);
    }
}
