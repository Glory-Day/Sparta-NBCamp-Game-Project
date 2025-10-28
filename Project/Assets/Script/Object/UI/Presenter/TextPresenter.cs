using Backend.Util.Presentation;
using Backend.Util.Presentation.Message;
using Script.Object.UI;

namespace Backend.Object.UI.Presenter
{
    public abstract class TextPresenter : Presenter<TextView, Inventory>, ISubscriber
    {
        private Dispatcher _dispatcher;
        public TextPresenter(TextView view, Inventory model, Dispatcher dispatcher) : base(view, model)
        {
            _dispatcher = dispatcher;
            _dispatcher.Subscribe(this);
        }

        public override void Clear()
        {
            base.Clear();
            _dispatcher.Unsubscribe(this);
        }

        public abstract void Receive<T>(T message);
    }
}
