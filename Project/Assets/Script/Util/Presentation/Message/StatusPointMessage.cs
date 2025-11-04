using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Backend.Util.Presentation.Message
{
    public enum StatusType
    {
        None,
        Level,
        Life,
        Concentration,
        Endurance,
        Health,
        Strength,
        Intelligence,
        Luck,
        MaximumHealth,
        MaximumStamina,
    }
    public struct StatusPointMessage
    {
        public StatusType StatusType;

        public StatusPointMessage(StatusType status)
        {
            StatusType = status;
        }
    }
}
