using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Backend.Object.UI;
using Test.Data;
using Test.Item.Base;
using UnityEngine;
using Debugger = Backend.Util.Debug.Debugger;

// 날짜 : 2021-03-28 PM 11:06:16
// 작성자 : Rito

namespace Test.Item
{
    /// <summary> 장비 - 방어구 아이템 </summary>
    public class ArmorItem : EquipmentItem, IUsableItem
    {
        public ArmorItem(ArmorItemData data) : base(data) { }

        public bool Use()
        {
            IsEquipped = !IsEquipped;

            Debugger.LogMessage($"방어구 {(IsEquipped ? "착용" : "해제")}");
            return IsEquipped;
        }
    }
}
