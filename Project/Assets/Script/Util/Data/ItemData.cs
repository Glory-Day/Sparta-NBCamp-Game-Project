using UnityEngine;

namespace Backend.Util.Data
{
    public class ItemData : ScriptableObject
    {
        [field: SerializeField] public int ID { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: Multiline]
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public int Priority { get; private set; }
    }
}
