using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Backend.Util.Presentation.Message
{
    public struct ConfirmMessage
    {
        public int Index;
        public int Point;

        public ConfirmMessage(int index, int point)
        {
            Index = index;
            Point = point;
        }
    }
}
