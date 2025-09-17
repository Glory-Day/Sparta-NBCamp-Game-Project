using System.Collections;
using System.Collections.Generic;
using Backend.Util.Data;
using UnityEngine;

namespace Backend.Util.Items
{
    [System.Serializable]
    public abstract class Item
    {
        public ItemData Data { get; private set; }

        public Item(ItemData data) => Data = data;
    }
}

