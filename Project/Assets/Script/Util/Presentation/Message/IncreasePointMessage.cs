using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Backend.Util.Presentation.Message
{
    public struct IncreasePointMessage
    {
        public int Index;
        public int Point;

        public IncreasePointMessage(int index, int point)
        {
            Index = index;
            Point = point;
        }
    }
}
