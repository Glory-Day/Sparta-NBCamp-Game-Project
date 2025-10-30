using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Backend.Object.UI
{
    public class EquipmentView : InventoryTestView
    {
        private Action EquipAction;
        private void PressLeftMouseButton(InputAction.CallbackContext context)
        {
            base.PressLeftMouseButton(context);
        }
    }
}
