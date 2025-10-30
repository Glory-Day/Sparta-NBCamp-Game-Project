using System.Collections;
using System.Collections.Generic;
using Backend.Object.Character.Player;
using Backend.Object.UI.View;
using Backend.Util.Presentation;
using Script.Object.UI.View;
using UnityEngine;

namespace Backend.Object.UI.Presenter
{
    public class PointTextPresenter : Presenter<PointTextView, PlayerStatus>
    {
        private int _index;
        public PointTextPresenter(PointTextView view, PlayerStatus model, int index) : base(view, model)
        {
            _index = index;
            Model.PointChanged[index] += OnPointChanged;
        }

        public override void Clear()
        {
            Model.PointChanged[_index] -= OnPointChanged;
            base.Clear();
        }

        private void OnPointChanged(int point)
        {
            View.Change(point);
        }
    }
}
