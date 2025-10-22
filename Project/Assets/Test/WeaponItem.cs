using System;
using System.Collections;
using System.Collections.Generic;
using Backend.Util.Debug;
using Test.Data;
using Test.Item.Base;
using UnityEngine;

// 날짜 : 2021-03-28 PM 11:02:03
// 작성자 : Rito

namespace Test.Item
{
    /// <summary> 장비 - 무기 아이템 </summary>
    public class WeaponItem : EquipmentItem, IUsableItem
    {
        public WeaponItem(WeaponItemData data) : base(data) { }

        public bool Use()
        {

            IsEquipped = !IsEquipped;

            Debugger.LogMessage($"무기 {(IsEquipped ? "착용" : "해제")}");
            return IsEquipped;
        }
    }
}
