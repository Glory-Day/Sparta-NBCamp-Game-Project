using System.Collections;
using System.Collections.Generic;
using Backend.Object.Character.Player;
using Backend.Object.UI.View;
using Backend.Util.Presentation;
using Script.Object.UI.View;
using UnityEngine;

namespace Backend.Object.UI.Presenter
{
    public abstract class PointTextPresenter : Presenter<PointTextView, PlayerStatus>, ISubscriber
    {
        Dispatcher _dispatcher;
        public PointTextPresenter(PointTextView view, PlayerStatus model, Dispatcher dispatcher) : base(view, model)
        {
            _dispatcher = dispatcher;
            _dispatcher.Subscribe(this);
        }

        public override void Clear()
        {
            base.Clear();
            _dispatcher.Unsubscribe(this);
        }

        public abstract void Receive<T>(T message);

        private void OnPointChanged(int point)
        {
            View.Change(point);
        }
    }
}
