using System.Collections;
using System.Collections.Generic;
using Backend.Util.Presentation;
using Backend.Util.Presentation.Message;
using Script.Object.UI;
using UnityEngine;

namespace Backend.Object.UI.Presenter
{
    public class ItemDescriptPresenter : TextPresenter
    {
        public ItemDescriptPresenter(TextView view, Inventory model, Dispatcher dispatcher) : base(view, model, dispatcher)
        {
        }

        public override void Receive<T>(T message)
        {
            switch (message)
            {
                case InventoryPointMessage msg:
                    View.Change(Model.items[msg.Index].Description);
                    break;
            }
        }
    }
}
