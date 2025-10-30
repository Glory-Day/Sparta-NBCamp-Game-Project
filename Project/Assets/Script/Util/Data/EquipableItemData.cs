using UnityEngine;

namespace Backend.Util.Data
{
    public class EquipableItemData : ItemData
    {
        [field: Header("Equipment Information")]
        [field: SerializeField] public bool IsEquipped { get; private set; }
    }
}
