using System.Collections;
using System.Collections.Generic;
using Backend.Util.Data;
using Backend.Util.Items;
using UnityEngine;

namespace Backend.Object.UI
{
    public class Inventory : MonoBehaviour
    {
        /// <summary> 아이템 수용 한도 </summary>
        public int Capacity { get; private set; }

        // 초기 수용 한도
        [SerializeField, Range(8, 64)]
        private int _initalCapacity = 32;

        // 최대 수용 한도(아이템 배열 크기)
        [SerializeField, Range(8, 64)]
        private int _maxCapacity = 64;

        [SerializeField]
        private InventoryUI _inventoryUI; // 연결된 인벤토리 UI


        [Header("테스트용 아이템")]
        [SerializeField] private PortionItem[] _testItemData;

        /// <summary> 아이템 목록 </summary>
        [SerializeField]
        private Item[] _items;

        private void Awake()
        {
            _items = new Item[_maxCapacity];
            Capacity = _initalCapacity;
            _inventoryUI.SetInventoryReference(this);
        }

        private void Start()
        {
            //UpdateAccessibleStatesAll();
        }

        /// <summary> 인덱스가 수용 범위 내에 있는지 검사 </summary>
        private bool IsValidIndex(int index)
        {
            return index >= 0 && index < Capacity;
        }

        /// <summary> 앞에서부터 비어있는 슬롯 인덱스 탐색 </summary>
        private int FindEmptySlotIndex(int startIndex = 0)
        {
            for (int i = startIndex; i < Capacity; i++)
                if (_items[i] == null)
                    return i;
            return -1;
        }

        /// <summary> 모든 슬롯 UI에 접근 가능 여부 업데이트 </summary>
        //public void UpdateAccessibleStatesAll()
        //{
        //    _inventoryUI.SetAccessibleSlotRange(Capacity);
        //}

        /// <summary> 해당 슬롯이 아이템을 갖고 있는지 여부 </summary>
        public bool HasItem(int index)
        {
            return IsValidIndex(index) && _items[index] != null;
        }

        /// <summary> 해당 슬롯이 셀 수 있는 아이템인지 여부 </summary>
        public bool IsCountableItem(int index)
        {
            return HasItem(index) && _items[index] is CountableItem;
        }

        /// <summary>
        /// 해당 슬롯의 현재 아이템 개수 리턴
        /// <para/> - 잘못된 인덱스 : -1 리턴
        /// <para/> - 빈 슬롯 : 0 리턴
        /// <para/> - 셀 수 없는 아이템 : 1 리턴
        /// </summary>
        public int GetCurrentAmount(int index)
        {
            if (!IsValidIndex(index))
                return -1;
            if (_items[index] == null)
                return 0;

            CountableItem ci = _items[index] as CountableItem;
            if (ci == null)
                return 1;

            return ci.Amount;
        }

        /// <summary> 해당 슬롯의 아이템 정보 리턴 </summary>
        public ItemData GetItemData(int index)
        {
            if (!IsValidIndex(index))
                return null;
            if (_items[index] == null)
                return null;

            return _items[index].Data;
        }

        /// <summary> 해당 슬롯의 아이템 이름 리턴 </summary>
        public string GetItemName(int index)
        {
            if (!IsValidIndex(index))
                return "";
            if (_items[index] == null)
                return "";

            return _items[index].Data.Name;
        }
    }
}
