using Backend.Object.Character.Player;
using Backend.Util.Data;
using Backend.Util.Presentation;
using Backend.Util.Presentation.Message;
using Script.Object.UI.View;

namespace Backend.Object.UI
{
    public class LevelPointDifferenceTextPresenter : PointDifferenceTextPresenter
    {
        private Dispatcher _dispatcher;
        private int _totalLevel;
        public LevelPointDifferenceTextPresenter(PointDifferenceTextView view, PlayerStatus model, int index, Dispatcher dispatcher) : base(view, model, index)
        {
            _dispatcher = dispatcher;
            _dispatcher.Subscribe(this);
            _totalLevel = 0;
        }

        public override void Clear()
        {
            base.Clear();
            _dispatcher.Unsubscribe(this);
        }

        public override void Receive<T>(T message)
        {
            var statusData = (PlayerStatusData)Model.data;

            switch (message)
            {
                case IncreasePointMessage msg:
                    _totalLevel += msg.Point;
                    View.Change(statusData.Level, statusData.Level + _totalLevel);
                    break;
                case ConfirmMessage msg:
                    statusData.Level += _totalLevel;
                    View.Change(statusData.Level);
                    _totalLevel = 0;
                    break;
            }
        }
    }
}
