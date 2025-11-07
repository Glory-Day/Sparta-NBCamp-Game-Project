using System.Collections;
using System.Collections.Generic;
using Backend.Object.Character.Player;
using Backend.Object.UI.View;
using Backend.Util.Data;
using Backend.Util.Presentation;
using Backend.Util.Presentation.Message;
using Script.Object.UI.View;
using UnityEngine;

namespace Backend.Object.UI.Presenter
{
    public class StaminaPointDifferenceTextPresenter : PointDifferenceTextPresenter
    {
        public StaminaPointDifferenceTextPresenter(PointDifferenceTextView view, PlayerStatus model, Dispatcher dispatcher) : base(view, model, dispatcher)
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
            var status = (PlayerStatusData)((PlayerStatus)Model).data;

            switch (message)
            {
                case IncreasePointMessage msg:
                    View.Change((int)status.StaminaPoint, (int)status.StaminaPoint + msg.Point);
                    break;
                case ConfirmMessage msg:
                    status.StaminaPoint = float.Parse(View.UpdatedPointText.text);
                    View.Change((int)status.StaminaPoint);
                    break;
            }
        }
        public override void StateChange()
        {
            base.StateChange();
            if (Model is PlayerStatus playerStatus)
            {
                var status = (PlayerStatusData)playerStatus.data;
                View.Change((int)status.StaminaPoint, (int)status.StaminaPoint);
            }
        }
    }
}
