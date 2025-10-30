using Backend.Object.Character.Player;
using Backend.Object.UI.View;
using Backend.Util.Presentation;

namespace Script.Object.UI
{
    public class StaminaPointBarPresenter : Presenter<PointBarView, PlayerStatus>
    {
        public StaminaPointBarPresenter(PointBarView view, PlayerStatus model) : base(view, model)
        {
            Model.StaminaPointChanged += OnStaminaPointChanged;
        }

        public override void Clear()
        {
            Model.StaminaPointChanged -= OnStaminaPointChanged;

            base.Clear();
        }

        private void OnStaminaPointChanged(float point)
        {
            View.Change(point);
        }
    }
}
