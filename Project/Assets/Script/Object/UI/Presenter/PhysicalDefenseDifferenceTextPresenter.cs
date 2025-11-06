using Backend.Object.Character.Player;
using Backend.Util.Data;
using Backend.Util.Presentation;
using Backend.Util.Presentation.Message;
using Script.Object.UI.View;
using UnityEngine.Rendering;

namespace Backend.Object.UI.Presenter
{
    public class PhysicalDefenseDifferenceTextPresenter : PointDifferenceTextPresenter
    {
        public PhysicalDefenseDifferenceTextPresenter(PointDifferenceTextView view, PlayerStatus model, Dispatcher dispatcher) : base(view, model, dispatcher)
        {

        }

        public override void Clear()
        {
            base.Clear();
        }

        public override void Receive<T>(T message)
        {
            if (Model is PlayerStatus playerStatus)
            {
                var status = (PlayerStatusData)playerStatus.data;
                switch (message)
                {
                    case InventoryPointMessage msg:
                        View.Change((int)status.PhysicalDefense, (int)(status.PhysicalDefense + msg.Point));
                        break;
                    case ConfirmMessage msg:
                        status.PhysicalDefense = float.Parse(View.UpdatedPointText.text);
                        View.Change((int)status.PhysicalDefense);
                        break;
                }
            }
        }
    }
}
