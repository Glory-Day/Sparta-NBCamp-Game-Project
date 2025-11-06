using System.Collections;
using System.Collections.Generic;
using Backend.Object.Character.Player;
using Backend.Object.UI.View;
using Backend.Util.Data;
using Backend.Util.Debug;
using Backend.Util.Presentation;
using Backend.Util.Presentation.Message;
using Script.Object.UI.View;
using UnityEngine;

namespace Backend.Object.UI.Presenter
{
    public class LifePointDifferenceTextPresenter : PointDifferenceTextPresenter
    {
        public LifePointDifferenceTextPresenter(PointDifferenceTextView view, PlayerStatus model, Dispatcher dispatcher) : base(view, model, dispatcher)
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
                        var difference = int.Parse(View.UpdatedPointText.text) - (int)status.LifePoint;
                        _dispatcher.DispatchTo<LevelPointDifferenceTextPresenter, IncreasePointMessage>(0, new IncreasePointMessage(0, -difference));

                        View.Change((int)status.LifePoint, (int)status.LifePoint + msg.Point);
                        _dispatcher.DispatchTo<HealthPointDifferenceTextPresenter, T>(0, message);

                        _dispatcher.DispatchTo<LevelPointDifferenceTextPresenter, IncreasePointMessage>(0, new IncreasePointMessage(0, msg.Point));
                        break;
                    case InventoryPointMessage msg:
                        View.Change((int)status.LifePoint, (int)status.LifePoint + msg.Point);
                        break;
                    case ConfirmMessage msg:
                        status.LifePoint = float.Parse(View.UpdatedPointText.text);
                        View.Change((int)status.LifePoint);
                        _dispatcher.DispatchTo<HealthPointDifferenceTextPresenter, T>(0, message);
                        _dispatcher.DispatchTo<LevelPointDifferenceTextPresenter, T>(0, message);
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
                View.Change((int)status.LifePoint, (int)status.LifePoint);
            }
        }
    }
}
