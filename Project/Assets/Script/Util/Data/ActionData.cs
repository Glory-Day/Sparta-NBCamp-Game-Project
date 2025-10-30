using UnityEngine;

namespace Backend.Util.Data
{
    public enum DamageType
    {
        PhysicalDamage,
        MagicalDamage
    }
    public class ActionData : ScriptableObject
    {
        [field: Header("Default Settings")]
        [field: SerializeField] public string ID { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public float Damage { get; private set; }
        [field: SerializeField] public DamageType DamageType { get; private set; }

        [field: Header("Audio Settings")]
        [field: SerializeField] public AudioClip ActionSound { get; private set; }
    }
}
