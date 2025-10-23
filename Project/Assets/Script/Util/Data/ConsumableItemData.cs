using UnityEngine;

namespace Backend.Util.Data
{
    [CreateAssetMenu(fileName = "Consumable_Item_Data", menuName = "Scriptable Object/Data/Consumable Item Data")]
    public class ConsumableItemData : ItemData
    {
        [field: Header("Condition Point Settings")]
        [field: SerializeField] public float HealthPoint { get; private set; }
        [field: SerializeField] public float StaminaPoint { get; private set; }

        [field: Header("Status Point Settings")]
        [field: SerializeField] public float LifePoint { get; private set; }
        [field: SerializeField] public float ConcentrationPoint { get; private set; }
        [field: SerializeField] public float EndurancePoint { get; private set; }
        [field: SerializeField] public float PhysicalPoint { get; private set; }
        [field: SerializeField] public float StrengthPoint { get; private set; }
        [field: SerializeField] public float IntelligencePoint { get; private set; }
        [field: SerializeField] public float LuckPoint { get; private set; }
    }
}
