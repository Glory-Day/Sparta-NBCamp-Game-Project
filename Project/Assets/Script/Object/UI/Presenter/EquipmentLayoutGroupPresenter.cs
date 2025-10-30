using System.Collections;
using System.Collections.Generic;
using Backend.Object.UI.View;
using Backend.Util.Presentation;
using Backend.Util.Presentation.Message;
using UnityEngine;

namespace Backend.Object.UI.Presenter
{
    public class EquipmentLayoutGroupPresenter : LayoutGroupPresenter
    {
        public EquipmentLayoutGroupPresenter(ActiveView view, Inventory model, Dispatcher dispatcher) : base(view, model, dispatcher)
        {

        }

        public override void Receive<T>(T message)
        {
            switch (message)
            {
                case LayoutActiveMessage msg:
                    View.Change(msg.Value);
                    break;
            }
        }
    }
}
