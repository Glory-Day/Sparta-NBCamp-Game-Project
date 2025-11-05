using System;
using System.Collections;
using System.Collections.Generic;
using Backend.Util.Presentation.Message;
using UnityEngine;

namespace Backend.Object.UI.View
{
    public class BattleSlotView : ImageView
    {
        public Action OnSlotAction;
        public BattleSlotType SlotType;

        private void LateUpdate()
        {
            OnSlotAction?.Invoke();
        }
    }

    public enum BattleSlotType
    {
        LeftWeaponSlot,
        RightWeaponSlot,
        ConsumeItemSlot,
    }
}
