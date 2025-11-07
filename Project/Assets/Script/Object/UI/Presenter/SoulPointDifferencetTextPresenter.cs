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
    public class SoulPointDifferencetTextPresenter : PointDifferenceTextPresenter
    {
        private int _totalValue;
        public SoulPointDifferencetTextPresenter(PointDifferenceTextView view, IModel model, Dispatcher dispatcher) : base(view, model, dispatcher)
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
                        View.Change((int)status.Soul, (int)status.Soul - msg.Point * 100);
                        break;
                    case ConfirmMessage msg:
                        // 증가하는 수치
                        var diff = float.Parse(View.UpdatedPointText.text);

                        if (diff < 0)
                        {
                            return;
                        }

                        status.Soul = float.Parse(View.UpdatedPointText.text);
                        View.Change((int)status.Soul);

                        _dispatcher.DispatchTo<LifePointDifferenceTextPresenter, ConfirmMessage>(0, new ConfirmMessage());
                        _dispatcher.DispatchTo<EndurancePointDifferenceTextPresenter, ConfirmMessage>(0, new ConfirmMessage());

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
                View.Change((int)status.Soul, (int)status.Soul);
            }
        }
    }
}
