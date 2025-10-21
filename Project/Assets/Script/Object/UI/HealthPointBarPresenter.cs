using System;
using Backend.Object.Character;
using Backend.Object.UI.View;
using Backend.Util.Presentation;
using UnityEngine;

namespace Backend.Object.UI
{
    public class HealthPointBarPresenter : Presenter<DelayedPointBarView, Status>
    {
        public HealthPointBarPresenter(DelayedPointBarView view, Status model) : base(view, model)
        {
            Model.HealthPointChanged += OnHealthPointChanged;
        }

        public override void Clear()
        {
            Model.HealthPointChanged -= OnHealthPointChanged;

            base.Clear();
        }

        private void OnHealthPointChanged(float value)
        {
            View.Change(value);
        }
    }
}
