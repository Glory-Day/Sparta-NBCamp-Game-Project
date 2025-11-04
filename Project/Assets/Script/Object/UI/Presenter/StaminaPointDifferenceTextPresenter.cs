using System.Collections;
using System.Collections.Generic;
using Backend.Object.Character.Player;
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
        }

        public override void Clear()
        {
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
            _dispatcher.DispatchTo<LevelPointDifferenceTextPresenter, T>(0, message);
        }
    }
}
