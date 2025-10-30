using UnityEngine;

namespace Backend.Util.Data
{
    [CreateAssetMenu(fileName = "Item_Data", menuName = "Scriptable Object/Data/Item Data")]
    public class ItemData : ScriptableObject
    {
        #region CONSTANT FIELD API

        public const int MaximumStackCount = 99;

        #endregion

        [field: Header("Default Settings")]
        [field: SerializeField] public int ID { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: Multiline]
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public Sprite IconImage { get; private set; }
        [field: SerializeField] public int Price { get; private set; }
        [field: SerializeField] public int Priority { get; private set; }
        [field: SerializeField] public int Count { get; set; }
        [field: SerializeField] public bool IsCountable { get; private set; }

        public int Sum(int count)
        {
            Count += count;

            return Count > MaximumStackCount ? Count - MaximumStackCount : 0;
        }

        public bool IsFull => Count == MaximumStackCount;

        public bool IsEmpty => Count == 0;
    }
}
