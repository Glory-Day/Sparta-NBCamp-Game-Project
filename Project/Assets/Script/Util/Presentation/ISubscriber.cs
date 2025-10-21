namespace Backend.Util.Presentation
{
    public interface ISubscriber
    {
        public void Receive<T>(T message);
    }
}
