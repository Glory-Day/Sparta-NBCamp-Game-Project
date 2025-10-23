using UnityEngine;

namespace Backend.Util.Data
{
    [CreateAssetMenu(fileName = "Armor_Item_Data", menuName = "Scriptable Object/Data/Armor Item Data")]
    public class ArmorItemData : EquipableItemData
    {
        [field: Header("Defense Point Settings")]
        [field: SerializeField] public float PhysicalDefensePoint { get; set; }
        [field: SerializeField] public float MagicalDefensePoint { get; set; }
    }
}
