using Backend.Object.Character.Player;
using Backend.Object.UI.View;
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
            if (View is StatusDifferenceTextView statusView)
            {
                statusView.UpdateState += StateChange;
            }
        }

        public override void Clear()
        {
            if (View is StatusDifferenceTextView statusView)
            {
                statusView.UpdateState -= StateChange;
            }
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
                        var difference = int.Parse(View.UpdatedPointText.text) - (int)status.EndurancePoint;
                        _dispatcher.DispatchTo<LevelPointDifferenceTextPresenter, IncreasePointMessage>(0, new IncreasePointMessage(0, -difference));

                        View.Change((int)status.EndurancePoint, (int)status.EndurancePoint + msg.Point);
                        _dispatcher.DispatchTo<StaminaPointDifferenceTextPresenter, T>(0, message);

                        _dispatcher.DispatchTo<LevelPointDifferenceTextPresenter, IncreasePointMessage>(0, new IncreasePointMessage(0, msg.Point));
                        _dispatcher.DispatchTo<SoulPointDifferencetTextPresenter, IncreasePointMessage>(0, new IncreasePointMessage(0, msg.Point));
                        break;
                    case ConfirmMessage msg:
                        status.EndurancePoint = float.Parse(View.UpdatedPointText.text);
                        View.Change((int)status.EndurancePoint);
                        _dispatcher.DispatchTo<StaminaPointDifferenceTextPresenter, T>(0, message);
                        _dispatcher.DispatchTo<LevelPointDifferenceTextPresenter, T>(0, message);
                        break;
                    case InventoryPointMessage msg:
                        View.Change((int)status.EndurancePoint, (int)status.EndurancePoint + msg.Point);
                        break;
                }
            }
        }
        public override void StateChange()
        {
            if (Model is PlayerStatus playerStatus)
            {
                var status = (PlayerStatusData)playerStatus.data;
                View.Change((int)status.EndurancePoint, (int)status.EndurancePoint);
            }
        }
    }
}

