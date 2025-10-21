using Backend.Object.Character.Player;
using Backend.Util.Presentation;
using Script.Object.UI.View;
using UnityEngine;

namespace Script.Object.UI
{
    public class PointPresenter : Presenter<PointTextView, PlayerStatus>
    {
        public PointPresenter(PointTextView view, PlayerStatus model) : base(view, model)
        {

        }

        public override void Clear()
        {

        }

        private void OnPointChanged(float point)
        {
            var text = Mathf.FloorToInt(point).ToString();

            View.Change(text);
        }
    }
}
