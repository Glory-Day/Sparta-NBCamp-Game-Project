using System;
using Backend.Util.Debug;
using UnityEngine.InputSystem;

namespace Backend.Object.UI.View
{
    public class EquipmentView : BaseInventoryView
    {
        public Action<int> EquipAction;
        public Action PopupAction;
        protected sealed override void PressLeftMouseButton(InputAction.CallbackContext context)
        {
            base.PressLeftMouseButton(context);
            if(_selectedSlot != null)
            {
                Debugger.LogMessage($"EmptySlot Select. You Can Choose Item. Slot : {_selectedSlot}");
                EquipAction.Invoke(_selectedSlot.Index);
            }
        }
    }
}
