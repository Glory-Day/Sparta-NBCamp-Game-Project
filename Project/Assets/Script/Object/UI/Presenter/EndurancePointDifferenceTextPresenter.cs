using Backend.Object.Character.Player;
using Backend.Util.Data;
using Backend.Util.Presentation;
using Backend.Util.Presentation.Message;
using Script.Object.UI.View;

namespace Backend.Object.UI.Presenter
{
    public class EndurancePointDifferenceTextPresenter : PointDifferenceTextPresenter
    {
        private Dispatcher _dispatcher;
        public EndurancePointDifferenceTextPresenter(PointDifferenceTextView view, PlayerStatus model, int index, Dispatcher dispatcher) : base(view, model, index)
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
                    View.Change((int)((PlayerStatusData)Model.data).EndurancePoint, ((int)((PlayerStatusData)Model.data).EndurancePoint) + msg.Point);
                    _dispatcher.DispatchTo<StaminaPointDifferenceTextPresenter, T>(message);
                    break;
                case ConfirmMessage msg:
                    ((PlayerStatusData)Model.data).EndurancePoint = float.Parse(View.UpdatedPointText.text);
                    View.Change((int)((PlayerStatusData)Model.data).EndurancePoint);
                    _dispatcher.DispatchTo<StaminaPointDifferenceTextPresenter, T>(message);
                    break;
                case InventoryPointMessage msg:
                    View.Change((int)((PlayerStatusData)Model.data).EndurancePoint, ((int)((PlayerStatusData)Model.data).EndurancePoint) + msg.Point);
                    break;
            }
        }
    }
}

