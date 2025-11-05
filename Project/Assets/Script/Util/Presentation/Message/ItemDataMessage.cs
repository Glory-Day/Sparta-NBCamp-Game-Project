using System.Collections;
using System.Collections.Generic;
using Backend.Util.Data;
using UnityEngine;

namespace Backend.Util.Presentation.Message
{
    public enum InventoryType
    {
        None,
        WeaponInventory,
        ArmorInventory,
        ConsumableInventory,
        AccessoryInventory,
    }
    public struct ItemDataMessage
    {
        public int ItemIndex;
        public ItemData Item;
        public InventoryType InvType;

        public ItemDataMessage(int itemIndex, InventoryType inventoryType = 0, ItemData item = null)
        {
            ItemIndex = itemIndex;
            Item = item;
            InvType = inventoryType;
        }
    }
}
