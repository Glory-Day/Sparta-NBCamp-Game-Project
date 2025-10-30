using UnityEngine;

namespace Backend.Util.Data
{
    public class StatusData : ScriptableObject
    {
        [field: Header("Default Settings")]
        [field: SerializeField] public string ID { get; protected set; }
        [field: SerializeField] public string Name { get; protected set; }
        [field: SerializeField] public float HealthPoint { get; set; }
        [field: SerializeField] public float Speed { get; set; }

        [field: Header("Attack Point Settings")]
        [field: SerializeField] public float PhysicalDamage { get; set; }
        [field: SerializeField] public float MagicalDamage { get; set; }

        [field: Header("Defense Point Settings")]
        [field: SerializeField] public float PhysicalDefense { get; set; }
        [field: SerializeField] public float MagicalDefense { get; set; }
    }
}
