using UnityEngine;

namespace Backend.Util.Data
{
    [CreateAssetMenu(fileName = "Weapon_Item_Data", menuName = "Scriptable Object/Data/Weapon Item Data")]
    public class WeaponItemData : EquipableItemData
    {
        [field: Header("Damage Type")]
        [field: SerializeField] public DamageType Type { get; set; }

        [field: Header("Damage Point Settings")]
        [field: SerializeField] public float PhysicalDamagePoint { get; set; }
        [field: SerializeField] public float MagicalDamagePoint { get; set; }

        public float DamagePoint => Type switch { DamageType.Magical => MagicalDamagePoint, DamageType.Physical => PhysicalDamagePoint, _ => throw new System.NotImplementedException() };

        public enum DamageType
        {
            Magical,
            Physical,
        }
    }
}
