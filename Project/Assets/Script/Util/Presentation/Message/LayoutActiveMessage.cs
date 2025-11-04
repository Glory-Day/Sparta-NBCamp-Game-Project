using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Backend.Object.UI.View;

namespace Backend.Util.Presentation.Message
{
    public enum LayoutType
    {
        None,
        EquipInventory,
        PlayerInventory,
        EquipInfo,
        ConsumeInfo,
        Popup,
    }

    public struct LayoutActiveMessage
    {
        public bool Value;
        public LayoutType Layout;
        public LayoutActiveMessage(bool value, LayoutType layoutType)
        {
            Value = value;
            Layout = layoutType;
        }
    }
}
