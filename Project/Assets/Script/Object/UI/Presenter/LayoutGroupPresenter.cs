﻿using System.Collections;
using System.Collections.Generic;
using Backend.Object.UI.View;
using Backend.Util.Presentation;
using Backend.Util.Presentation.Message;
using UnityEngine;


namespace Backend.Object.UI.Presenter
{
    public abstract class LayoutGroupPresenter : Presenter<ActiveView, Inventory>, ISubscriber
    {
        private Dispatcher _dispatcher;
        public LayoutGroupPresenter(ActiveView view, Inventory model, Dispatcher dispatcher) : base(view, model)
        {
            _dispatcher = dispatcher;
            _dispatcher.Subscribe(this);
        }

        public override void Clear()
        {
            base.Clear();
            _dispatcher.Unsubscribe(this);
        }

        public abstract void Receive<T>(T message);
    }
}
