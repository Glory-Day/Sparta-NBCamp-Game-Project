using System.Collections;
using System.Collections.Generic;
using Backend.Object.Character.Player;
using Backend.Util.Presentation;
using Script.Object.UI.View;
using UnityEngine;
using static UnityEngine.CullingGroup;

namespace Backend.Object.UI.Presenter
{
    public abstract class PointDifferenceTextPresenter : Presenter<PointDifferenceTextView, IModel>, ISubscriber
    {
        protected Dispatcher _dispatcher;
        public PointDifferenceTextPresenter(PointDifferenceTextView view, IModel model, Dispatcher dispatcher) : base(view, model)
        {
            _dispatcher = dispatcher;
            _dispatcher.Subscribe(this);
        }

        public override void Clear()
        {
            _dispatcher.Unsubscribe(this);
            base.Clear();
        }

        public abstract void Receive<T>(T message);

        private void OnPointChanged(int point)
        {
            View.Change(point);
        }

        public virtual void StateChange()
        {

        }
    }
}
