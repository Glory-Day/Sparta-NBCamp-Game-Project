using Backend.Object.Character.Player;
using Backend.Util.Data;
using Backend.Util.Presentation;
using Backend.Util.Presentation.Message;
using Script.Object.UI.View;

namespace Backend.Object.UI.Presenter
{
    public class ItemCountDifferenceTextPresenter : Presenter<PointDifferenceTextView, Inventory>, ISubscriber
    {
        private Dispatcher _dispatcher;
        public ItemCountDifferenceTextPresenter(PointDifferenceTextView view, Inventory model, Dispatcher dispatcher) : base(view, model)
        {
            _dispatcher = dispatcher;
            _dispatcher.Subscribe(this);
        }

        public override void Clear()
        {
            base.Clear();
            _dispatcher.Unsubscribe(this);
        }

        public void Receive<T>(T message)
        {
            switch (message)
            {
                case InventoryPointMessage msg:
                    View.Change(Model.items[msg.Index].Count, Model.items[msg.Index].Count);
                    break;
                case ConfirmMessage msg:
                    Model.items[msg.Index].Count = int.Parse(View.UpdatedPointText.text);
                    View.Change(Model.items[msg.Index].Count);
                    break;
            }
        }
    }
}
