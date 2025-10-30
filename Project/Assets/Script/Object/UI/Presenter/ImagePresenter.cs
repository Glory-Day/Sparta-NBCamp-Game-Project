using Backend.Object.UI.View;
using Backend.Util.Presentation;
using Backend.Util.Presentation.Message;

namespace Backend.Object.UI.Presenter
{
    public class ImagePresenter : Presenter<ImageView, Inventory>, ISubscriber
    {
        private Dispatcher _dispatcher;
        public ImagePresenter(ImageView view, Inventory model, Dispatcher dispatcher) : base(view, model)
        {
            _dispatcher = dispatcher;
            _dispatcher.Subscribe(this);
        }

        public void Receive<T>(T message)
        {
            switch (message)
            {
                case InventoryPointMessage msg:
                    View.Change(Model.items[msg.Index].IconImage);
                    break;
            }
        }
    }
}

