using System.Collections;
using System.Collections.Generic;
using Backend.Object.Character.Player;
using Backend.Util.Presentation;
using Script.Object.UI.View;
using UnityEngine;

namespace Backend.Object.UI
{
    public abstract class PointDifferenceTextPresenter : Presenter<PointDifferenceTextView, PlayerStatus>, ISubscriber
    {
        private int _index;
        public PointDifferenceTextPresenter(PointDifferenceTextView view, PlayerStatus model, int index) : base(view, model)
        {
            _index = index;
            Model.PointChanged[index] += OnPointChanged;
        }

        public override void Clear()
        {
            Model.PointChanged[_index] -= OnPointChanged;
            base.Clear();
        }

        public abstract void Receive<T>(T message);

        private void OnPointChanged(int point)
        {
            View.Change(point);
        }
    }
}
