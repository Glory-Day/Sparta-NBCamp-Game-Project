using Backend.Object.Character.Player;
using Backend.Util.Data;
using Backend.Util.Presentation;
using Backend.Util.Presentation.Message;
using Script.Object.UI.View;

namespace Backend.Object.UI.Presenter
{
    public class EndurancePointDifferenceTextPresenter : PointDifferenceTextPresenter
    {
        public EndurancePointDifferenceTextPresenter(PointDifferenceTextView view, PlayerStatus model, Dispatcher dispatcher) : base(view, model, dispatcher)
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
                    case IncreasePointMessage msg:
                        View.Change((int)status.EndurancePoint, (int)status.EndurancePoint + msg.Point);
                        _dispatcher.DispatchTo<StaminaPointDifferenceTextPresenter, T>(0, message);
                        break;
                    case ConfirmMessage msg:
                        status.EndurancePoint = float.Parse(View.UpdatedPointText.text);
                        View.Change((int)status.EndurancePoint);
                        _dispatcher.DispatchTo<StaminaPointDifferenceTextPresenter, T>(0, message);
                        break;
                    case InventoryPointMessage msg:
                        View.Change((int)status.EndurancePoint, (int)status.EndurancePoint + msg.Point);
                        break;
                }
            }
        }
    }
}

