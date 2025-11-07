using System.Collections;
using System.Collections.Generic;
using Backend.Object.Character.Player;
using Backend.Util.Data;
using Backend.Util.Debug;
using Backend.Util.Presentation;
using Backend.Util.Presentation.Message;
using UnityEngine;

namespace Backend.Object.UI.Presenter
{
    public class StatusUIPresenter : Presenter<PlayerMainStatusUI, PlayerStatus>
    {
        private Dispatcher _dispatcher;

        public StatusUIPresenter(PlayerMainStatusUI view, PlayerStatus model, Dispatcher dispatcher) : base(view, model)
        {
            _dispatcher = dispatcher;
            View.ConfirmButton.onClick.AddListener(() => OnAccept());
        }

        private void OnAccept()
        {
            var message = new ConfirmMessage();

            _dispatcher.DispatchTo<SoulPointDifferencetTextPresenter, ConfirmMessage>(0, message);
        }
    }
}
