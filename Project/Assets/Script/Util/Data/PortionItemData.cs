using System.Collections;
using System.Collections.Generic;
using Backend.Util.Items;
using UnityEngine;
using static UnityEditor.Progress;

namespace Backend.Util.Data
{
    /// <summary> 소비 아이템 정보 </summary>
    [CreateAssetMenu(fileName = "Item_Portion_", menuName = "Inventory System/Item Data/Portion", order = 3)]
    public class PortionItemData : CountableItemData
    {
        /// <summary> 효과량(회복량 등) </summary>
        public float Value => _value;
        [SerializeField] private float _value;
        public override Items.Item CreateItem()
        {
            return new PortionItem(this);
        }
    }
}
