using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Backend.Util.Presentation.Message
{
    public struct InventoryPointMessage
    {
        public int Index;
        public int Point;

        public InventoryPointMessage(int index, int point)
        {
            Index = index;
            Point = point;
        }
    }
}
