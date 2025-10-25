using System.Collections;
using System.Collections.Generic;
using Backend.Object.Character.Player;
using Backend.Util.Presentation;
using Script.Object.UI.View;
using UnityEngine;

namespace Backend.Object.UI
{
    public class PlayerLevelStatusInformationBinder : MonoBehaviour
    {
        [Header("View References")]
        [SerializeField] private PointDifferenceTextView[] pointDifferenceTextView;
        [SerializeField] private PlayerLevelUpPopupUI levelUpPopupUI;
        [SerializeField] private PlayerMainStatusUI mainStatusUI;

        private PointDifferenceTextPresenter[] _pointDifferenceTextPresenter;
        private PopupUIPresenter _popupUIPresenter;
        private StatusUIPresenter _statusUIPresenter;

        private Dispatcher _dispatcher = new();

        public void Bind(PlayerStatus model)
        {
            _pointDifferenceTextPresenter = new PointDifferenceTextPresenter[pointDifferenceTextView.Length];

            _pointDifferenceTextPresenter[0] = PresenterFactory.Create<LifePointDifferenceTextPresenter>(pointDifferenceTextView[0], model, 0, _dispatcher);
            _pointDifferenceTextPresenter[1] = PresenterFactory.Create<HealthPointDifferenceTextPresenter>(pointDifferenceTextView[1], model, 1, _dispatcher);
            _pointDifferenceTextPresenter[2] = PresenterFactory.Create<EndurancePointDifferenceTextPresenter>(pointDifferenceTextView[2], model, 2, _dispatcher);
            _pointDifferenceTextPresenter[3] = PresenterFactory.Create<StaminaPointDifferenceTextPresenter>(pointDifferenceTextView[3], model, 3, _dispatcher);
            _pointDifferenceTextPresenter[4] = PresenterFactory.Create<LevelPointDifferenceTextPresenter>(pointDifferenceTextView[4], model, 4, _dispatcher);

            _popupUIPresenter = PresenterFactory.Create<PopupUIPresenter>(levelUpPopupUI, null, _dispatcher);
            _statusUIPresenter = PresenterFactory.Create<StatusUIPresenter>(mainStatusUI, null, _dispatcher);
        }

        private void OnDestroy()
        {
            for (int i = 0; i < pointDifferenceTextView.Length; i++)
            {
                if (_pointDifferenceTextPresenter[i] != null)
                {
                    _pointDifferenceTextPresenter[i].Clear();
                }
            }
        }
    }
}
