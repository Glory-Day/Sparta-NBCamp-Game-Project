using Backend.Object.Character.Player;
using Backend.Object.UI;
using Backend.Object.UI.Presenter;
using Backend.Object.UI.View;
using Backend.Util.Presentation;
using UnityEngine;

namespace Script.Object.UI
{
    public class PlayerConditionInformationBinder : MonoBehaviour
    {
        [Header("View References")]
        [SerializeField] private DelayedPointBarView healthPointBarView;
        [SerializeField] private PointBarView staminaPointBarView;

        private HealthPointBarPresenter _healthPointBarPresenter;
        private StaminaPointBarPresenter _staminaPointBarPresenter;

        public void Bind(PlayerStatus model)
        {
            _healthPointBarPresenter = PresenterFactory.Create<HealthPointBarPresenter>(healthPointBarView, model);
            _staminaPointBarPresenter = PresenterFactory.Create<StaminaPointBarPresenter>(staminaPointBarView, model);
        }

        private void OnDestroy()
        {
            _healthPointBarPresenter.Clear();
            _staminaPointBarPresenter.Clear();
        }
    }
}
