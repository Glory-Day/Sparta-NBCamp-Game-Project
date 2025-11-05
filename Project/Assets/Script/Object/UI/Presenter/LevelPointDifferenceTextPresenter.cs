using Backend.Object.Character.Player;
using Backend.Util.Data;
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
            _totalLevel = 0;
        }

        public override void Clear()
        {
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
                        break;
                    case ConfirmMessage msg:
                        status.Level += _totalLevel;
                        View.Change(status.Level);
                        _totalLevel = 0;
                        break;
                }
            }
        }
    }
}
