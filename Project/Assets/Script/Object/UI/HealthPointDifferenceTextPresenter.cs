using Backend.Object.Character;
using Backend.Object.Character.Player;
using Backend.Util.Data;
using Backend.Util.Presentation;
using Backend.Util.Presentation.Message;
using Script.Object.UI.View;

namespace Backend.Object.UI
{
    public class HealthPointDifferenceTextPresenter : PointDifferenceTextPresenter
    {
        private Dispatcher _dispatcher;
        public HealthPointDifferenceTextPresenter(PointDifferenceTextView view, PlayerStatus model, int index, Dispatcher dispatcher) : base(view, model, index)
        {
            _dispatcher = dispatcher;
            _dispatcher.Subscribe(this);
        }

        public override void Clear()
        {
            base.Clear();
            _dispatcher.Unsubscribe(this);
        }

        public override void Receive<T>(T message)
        {
            switch (message)
            {
                case IncreasePointMessage msg:
                    View.Change((int)Model.maximumHealthPoint, (int)Model.maximumHealthPoint + msg.Point);
                    break;
                case ConfirmMessage msg:
                    Model.maximumHealthPoint = float.Parse(View.UpdatedPointText.text);
                    View.Change((int)Model.maximumHealthPoint);
                    break;
            }
            _dispatcher.DispatchTo<LevelPointDifferenceTextPresenter, T>(message);
        }
    }
}
