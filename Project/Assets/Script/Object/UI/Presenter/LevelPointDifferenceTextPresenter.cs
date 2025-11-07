using Backend.Object.Character.Player;
using Backend.Object.UI.View;
using Backend.Util.Data;
using Backend.Util.Debug;
using Backend.Util.Presentation;
using Backend.Util.Presentation.Message;
using Script.Object.UI.View;

namespace Backend.Object.UI.Presenter
{
    public class LevelPointDifferenceTextPresenter : PointDifferenceTextPresenter
    {
        private int _totalLevel;
        public LevelPointDifferenceTextPresenter(PointDifferenceTextView view, PlayerStatus model, Dispatcher dispatcher) : base(view, model, dispatcher)
        {
            if (View is StatusDifferenceTextView statusView)
            {
                statusView.UpdateState += StateChange;
            }
            _totalLevel = 0;
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
            if(Model is PlayerStatus playerStatus)
            {
                var status = (PlayerStatusData)playerStatus.data;
                switch (message)
                {
                    case IncreasePointMessage msg:
                        _totalLevel += msg.Point;
                        View.Change(status.Level, status.Level + _totalLevel);

                        var difference = int.Parse(View.UpdatedPointText.text) - (int)status.Level;
                        _dispatcher.DispatchTo<SoulPointDifferencetTextPresenter, IncreasePointMessage>(0, new IncreasePointMessage(0, difference));
                        break;
                    case ConfirmMessage msg:
                        status.Level = int.Parse(View.UpdatedPointText.text);
                        View.Change(status.Level);
                        _totalLevel = 0;
                        break;
                }
            }
        }

        public override void StateChange()
        {
            base.StateChange();
            if (Model is PlayerStatus playerStatus)
            {
                var status = (PlayerStatusData)playerStatus.data;
                View.Change(status.Level, status.Level);
            }
        }
    }
}
