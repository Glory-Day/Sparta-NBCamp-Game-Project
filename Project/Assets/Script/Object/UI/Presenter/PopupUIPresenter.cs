using System.Collections;
using System.Collections.Generic;
using Backend.Object.Character.Player;
using Backend.Util.Debug;
using Backend.Util.Presentation;
using Backend.Util.Presentation.Message;
using UnityEngine;

namespace Backend.Object.UI.Presenter
{
    public class PopupUIPresenter : Presenter<PlayerLevelUpPopupUI, IModel>
    {
        private Dispatcher _dispatcher;
        public PopupUIPresenter(PlayerLevelUpPopupUI view, IModel model, Dispatcher dispatcher) : base(view, model)
        {
            _dispatcher = dispatcher;
            View.amountInputOkButton.onClick.AddListener(() => OnAccept());
        }

        private void OnAccept()
        {
            var index = View.StatusIndex;
            var point = View.Point;

            var message = new IncreasePointMessage(index, point);

            switch (index)
            {
                case 0:
                    _dispatcher.DispatchTo<LifePointDifferenceTextPresenter, IncreasePointMessage>(message);
                    break;
                case 2:
                    _dispatcher.DispatchTo<EndurancePointDifferenceTextPresenter, IncreasePointMessage>(message);
                    break;
            }
        }
    }
}
