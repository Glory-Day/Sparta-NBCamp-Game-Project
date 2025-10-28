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
    public class PhysicalPointDifferenceTextPresenter : Presenter<PointDifferenceTextView, PlayerStatus>, ISubscriber
    {
        private Dispatcher _dispatcher;
        public PhysicalPointDifferenceTextPresenter(PointDifferenceTextView view, PlayerStatus model, Dispatcher dispatcher) : base(view, model)
        {
            _dispatcher = dispatcher;
            _dispatcher.Subscribe(this);
        }

        public override void Clear()
        {
            base.Clear();
            _dispatcher.Unsubscribe(this);
        }

        public void Receive<T>(T message)
        {
            switch (message)
            {
                case InventoryPointMessage msg:
                    View.Change((int)((PlayerStatusData)Model.data).PhysicalDamage, ((int)((PlayerStatusData)Model.data).PhysicalDamage) + msg.Point);
                    break;
                case ConfirmMessage msg:
                    ((PlayerStatusData)Model.data).PhysicalDamage = float.Parse(View.UpdatedPointText.text);
                    View.Change((int)((PlayerStatusData)Model.data).PhysicalDamage);
                    break;
            }
        }
    }
}
