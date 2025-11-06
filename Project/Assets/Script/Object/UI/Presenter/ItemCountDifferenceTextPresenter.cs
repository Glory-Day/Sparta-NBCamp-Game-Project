using Backend.Object.Character.Player;
using Backend.Util.Data;
using Backend.Util.Presentation;
using Backend.Util.Presentation.Message;
using Script.Object.UI.View;

namespace Backend.Object.UI.Presenter
{
    public class ItemCountDifferenceTextPresenter : PointDifferenceTextPresenter
    {
        public ItemCountDifferenceTextPresenter(PointDifferenceTextView view, Inventory model, Dispatcher dispatcher) : base(view, model, dispatcher)
        {
        }

        public override void Clear()
        {
            base.Clear();
        }

        public override void Receive<T>(T message)
        {
            var inv = (Inventory)Model;
            switch (message)
            {
                case InventoryPointMessage msg when Model is Inventory:
                    View.Change(inv.items[msg.Index].Count, inv.items[msg.Index].Count);
                    break;
                case ConfirmMessage msg:
                    inv.items[msg.Index].Count = int.Parse(View.UpdatedPointText.text);
                    View.Change(inv.items[msg.Index].Count);
                    break;
            }
        }
    }
}
