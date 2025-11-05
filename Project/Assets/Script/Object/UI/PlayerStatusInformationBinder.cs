using System.Collections;
using System.Collections.Generic;
using Backend.Object.Character.Player;
using Backend.Object.UI.Presenter;
using Backend.Object.UI.View;
using Backend.Util.Presentation;
using Script.Object.UI;
using UnityEngine;

namespace Backend.Object.UI
{
    public class PlayerStatusInformationBinder : MonoBehaviour
    {
        #region View Reference
        [Header("Main Panel")]
        [SerializeField] private StatusTextView[] mainTextView;
        [SerializeField] private TextView[] mainSoulTextView;


        [Header("Detail Panel")]
        [SerializeField] private StatusTextView[] detailTextView;
        [SerializeField] private WeaponCalculateView weaponCalculateTextView;
        [SerializeField] private ArmorCalculateView armorCalculateTextView;

        private StatusTextPresenter[] _mainTextPresenter;
        private StatusTextPresenter[] _detailTextPresenter;
        private WeaponCalculatePresenter _weaponCalculatePresenter;
        private ArmorCalculatePresenter _armorCalculatePresenter;

        private Dispatcher _dispatcher = new();

        #endregion

        private void Init()
        {
            _mainTextPresenter = new StatusTextPresenter[mainTextView.Length];
            _detailTextPresenter = new StatusTextPresenter[detailTextView.Length];
        }

        public void Bind(Inventory[] inventory, PlayerStatus status)
        {
            Init();

            for (var i = 0; i < mainTextView.Length; i++)
            {
                _mainTextPresenter[i] = PresenterFactory.Create<StatusTextPresenter>(mainTextView[i], status, _dispatcher);
            }
            for (var i = 0; i < detailTextView.Length; i++)
            {
                _detailTextPresenter[i] = PresenterFactory.Create<StatusTextPresenter>(detailTextView[i], status, _dispatcher);
            }

            _weaponCalculatePresenter = PresenterFactory.Create<WeaponCalculatePresenter>(weaponCalculateTextView, inventory[1], _dispatcher);
            _armorCalculatePresenter = PresenterFactory.Create<ArmorCalculatePresenter>(armorCalculateTextView, inventory[2], _dispatcher);
        }
    }
}
