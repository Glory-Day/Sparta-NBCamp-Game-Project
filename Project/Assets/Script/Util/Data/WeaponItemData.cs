using UnityEngine;

namespace Backend.Util.Data
{
    [CreateAssetMenu(fileName = "Weapon_Item_Data", menuName = "Scriptable Object/Data/Weapon Item Data")]
    public class WeaponItemData : EquipableItemData
    {
        [field: Header("Damage Point Settings")]
        [field: SerializeField] public float PhysicalDamagePoint { get; set; }
        [field: SerializeField] public float MagicalDamagePoint { get; set; }
    }
}
