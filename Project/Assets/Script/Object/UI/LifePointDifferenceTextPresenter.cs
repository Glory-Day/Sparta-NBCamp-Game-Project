using System.Collections;
using System.Collections.Generic;
using Backend.Object.Character.Player;
using Backend.Util.Data;
using Backend.Util.Presentation;
using Backend.Util.Presentation.Message;
using Script.Object.UI.View;
using UnityEngine;

namespace Backend.Object.UI
{
    public class LifePointDifferenceTextPresenter : PointDifferenceTextPresenter
    {
        private Dispatcher _dispatcher;
        public LifePointDifferenceTextPresenter(PointDifferenceTextView view, PlayerStatus model, int index, Dispatcher dispatcher) : base(view, model, index)
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
                    View.Change((int)((PlayerStatusData)Model.data).LifePoint, ((int)((PlayerStatusData)Model.data).LifePoint) + msg.Point);
                    break;
                case ConfirmMessage msg:
                    ((PlayerStatusData)Model.data).LifePoint = float.Parse(View.UpdatedPointText.text);
                    View.Change((int)((PlayerStatusData)Model.data).LifePoint);
                    break;
            }
            _dispatcher.DispatchTo<HealthPointDifferenceTextPresenter, T>(message);
        }
    }
}
