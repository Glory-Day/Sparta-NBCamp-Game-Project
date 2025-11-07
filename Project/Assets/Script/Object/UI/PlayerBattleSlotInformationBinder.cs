using System.Collections;
using System.Collections.Generic;
using Backend.Object.UI.Presenter;
using Backend.Object.UI.View;
using Backend.Util.Presentation;
using UnityEngine;

namespace Backend.Object.UI
{
    public class PlayerBattleSlotInformationBinder : MonoBehaviour
    {
        [SerializeField] private BattleSlotView[] battleSlotView;

        private BattleSlotImagePresenter[] _battleSlotImagePresenter;
        private void Init()
        {
            _battleSlotImagePresenter = new BattleSlotImagePresenter[battleSlotView.Length];
        }
        public void Binder(Inventory[] inventory)
        {
            Init();

            _battleSlotImagePresenter[0] = PresenterFactory.Create<BattleSlotImagePresenter>(battleSlotView[0], inventory[1]);
            _battleSlotImagePresenter[1] = PresenterFactory.Create<BattleSlotImagePresenter>(battleSlotView[1], inventory[1]);
            _battleSlotImagePresenter[2] = PresenterFactory.Create<BattleSlotImagePresenter>(battleSlotView[2], inventory[3]);
        }
    }
}
