using Backend.Object.Character;
using Backend.Object.Character.Player;
using Backend.Util.Data;
using Backend.Util.Presentation;
using Backend.Util.Presentation.Message;
using Script.Object.UI.View;

namespace Backend.Object.UI.Presenter
{
    public class HealthPointDifferenceTextPresenter : PointDifferenceTextPresenter
    {
        public HealthPointDifferenceTextPresenter(PointDifferenceTextView view, PlayerStatus model, Dispatcher dispatcher) : base(view, model, dispatcher)
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
                switch (message)
                {
                    case IncreasePointMessage msg:
                        View.Change((int)playerStatus.maximumHealthPoint, (int)playerStatus.maximumHealthPoint + msg.Point);
                        break;
                    case ConfirmMessage msg:
                        playerStatus.maximumHealthPoint = float.Parse(View.UpdatedPointText.text);
                        View.Change((int)playerStatus.maximumHealthPoint);
                        break;
                }
                _dispatcher.DispatchTo<LevelPointDifferenceTextPresenter, T>(0, message);
            }
        }
    }
}
