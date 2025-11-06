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
    public class PhysicalPointDifferenceTextPresenter : PointDifferenceTextPresenter
    {
        public PhysicalPointDifferenceTextPresenter(PointDifferenceTextView view, PlayerStatus model, Dispatcher dispatcher) : base(view, model, dispatcher)
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
                var status = ((PlayerStatusData)playerStatus.data).PhysicalDamage;

                switch (message)
                {
                    case InventoryPointMessage msg:
                        View.Change((int)status, (int)status + msg.Point);
                        break;
                    case ConfirmMessage msg:
                        status = float.Parse(View.UpdatedPointText.text);
                        View.Change((int)status);
                        break;
                }
            }
        }
    }
}
