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
            if (Model is Inventory inventory)
            {
                switch (message)
                {
                    case InventoryPointMessage msg:
                        View.Change(inventory.items[msg.Index].Description);
                        break;
                }
            }
        }
    }
}
