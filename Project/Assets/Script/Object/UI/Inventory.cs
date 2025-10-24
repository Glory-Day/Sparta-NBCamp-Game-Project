using System.Collections.Generic;
using Backend.Util.Data;
using UnityEngine;

namespace Backend.Object.UI
{
    public class Inventory : MonoBehaviour
    {
        /// <summary> 아이템 수용 한도 </summary>
        public int Capacity { get; private set; }

        // 초기 수용 한도
        [SerializeField, Range(8, 81)]
        private int _initalCapacity = 32;

        // 최대 수용 한도(아이템 배열 크기)
        [SerializeField, Range(8, 81)]
        private int _maxCapacity = 81;

        [SerializeField]
        private InventoryView inventoryView; // 연결된 인벤토리 UI

        /// <summary> 아이템 목록 </summary>
        [SerializeField]
        private ItemData[] items;

        /// <summary> 업데이트 할 인덱스 목록 </summary>
        private readonly HashSet<int> _indexSetForUpdate = new ();

        // 아이템 장착 여부 및 슬롯 번호
        private bool _isArmorEquip = false;
        private bool _isWeaponEquip = false;
        private int _currentArmorEquip;
        private int _currentWeaponEquip;

#if UNITY_EDITOR

        private void OnValidate()
        {
            if (_initalCapacity > _maxCapacity)
            {
                _initalCapacity = _maxCapacity;
            }
        }

#endif
        private void Awake()
        {
            items = new ItemData[_maxCapacity];
            Capacity = _initalCapacity;
            inventoryView.SetInventoryReference(this);
        }

        /// <summary> 인덱스가 수용 범위 내에 있는지 검사 </summary>
        private bool IsValidIndex(int index)
        {
            return index >= 0 && index < Capacity;
        }

        /// <summary>
        /// 앞에서부터 비어있는 슬롯 인덱스를 탐색한다.
        /// </summary>
        private int FindEmptySlotIndex(int startIndex = 0)
        {
            for (int i = startIndex; i < Capacity; i++)
            {
                if (items[i] == null)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// 앞에서부터 개수 여유가 있는 소모용 아이템의 슬롯 인덱스를 탐색한다.
        /// </summary>
        private int FindCountableItemSlotIndex(ItemData target, int startIndex = 0)
        {
            for (int i = startIndex; i < Capacity; i++)
            {
                var current = items[i];
                if (current == null)
                {
                    continue;
                }

                // 아이템 종류 일치 여부와 개수 여유를 확인하고 조건들을 만족할 경우에 해당 인덱스 번호를 반환한다.
                if (current == target && current.IsCountable && current.IsFull == false)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// 해당하는 인덱스의 슬롯 상태를 갱신한다.
        /// </summary>
        private void UpdateSlot(int index)
        {
            if (IsValidIndex(index) == false)
            {
                return;
            }

            var item = items[index];

            // 아이템이 슬롯에 존재하는 경우에는 다음과 같다.
            if (item != null)
            {
                // 아이콘 등록
                inventoryView.SetItemIconImageByIndex(index, item.IconImage);

                // 셀 수 있는 아이템인 경우는 다음과 같다.
                if (item.IsCountable)
                {
                    // 수량이 0인 경우, 아이템을 제거한다.
                    if (item.IsEmpty)
                    {
                        items[index] = null;
                        RemoveIconImage(index);
                        return;
                    }

                    // 수량 텍스트를 표시한다.
                    inventoryView.SetItemAmountTextByIndex(index, item.Count);
                }
                // 셀 수 없는 아이템인 경우 수량 텍스트 제거한다.
                else
                {
                    inventoryView.HideItemAmountText(index);
                }

                // 장착 가능 아이템일 경우에는 장착 여부를 표시한다.
                inventoryView.SetEquipmentText(index, item is EquipableItemData { IsEquipped: true });
            }
            // 빈 슬롯인 경우에는 아이콘 이미지를 제거한다.
            else
            {
                RemoveIconImage(index);
            }
        }

        private void RemoveIconImage(int index)
        {
            inventoryView.RemoveItem(index);
            inventoryView.HideItemAmountText(index);      // 수량 텍스트 숨기기
            inventoryView.SetEquipmentText(index, false); // 장착 여부 텍스트 숨기기
        }

        /// <summary>
        /// 해당하는 인덱스의 슬롯들의 상태를 갱신한다.
        /// </summary>
        private void UpdateSlot(params int[] indexes)
        {
            foreach (var index in indexes)
            {
                UpdateSlot(index);
            }
        }

        /// <summary> 모든 슬롯들의 상태를 UI에 갱신 </summary>
        private void UpdateAllSlot()
        {
            for (int i = 0; i < Capacity; i++)
            {
                UpdateSlot(i);
            }
        }

        /// <returns>
        /// 해당 슬롯이 아이템을 갖고 있는지 여부.
        /// </returns>
        public bool HasItem(int index)
        {
            return IsValidIndex(index) && items[index] != null;
        }

        /// <returns>
        /// 해당 슬롯이 셀 수 있는 아이템인지 여부.
        /// </returns>
        public bool IsCountableItem(int index)
        {
            return HasItem(index) && items[index].IsCountable;
        }

        /// <summary>
        /// 해당 슬롯의 현재 아이템 개수 리턴
        /// <para/> - 잘못된 인덱스 : -1 리턴
        /// <para/> - 빈 슬롯 : 0 리턴
        /// <para/> - 셀 수 없는 아이템 : 1 리턴
        /// </summary>
        public int GetCurrentAmount(int index)
        {
            if (IsValidIndex(index) == false)
            {
                return -1;
            }

            if (items[index] == null)
            {
                return 0;
            }

            return items[index].IsCountable == false ? 1 : items[index].Count;
        }

        /// <returns>
        /// 해당 슬롯의 아이템 데이터.
        /// </returns>
        public ItemData GetItemData(int index)
        {
            if (IsValidIndex(index) == false)
            {
                return null;
            }

            return items[index] == null ? null : items[index];
        }

        /// <summary> 해당 슬롯의 아이템 이름 리턴 </summary>
        public string GetItemName(int index)
        {
            if (!IsValidIndex(index))
            {
                return string.Empty;
            }

            return items[index] == null ? string.Empty : items[index].Name;
        }

        /// <summary> 인벤토리 UI 연결 </summary>
        public void ConnectUI(InventoryView view)
        {
            inventoryView = view;
            inventoryView.SetInventoryReference(this);
        }

        /// <summary> 인벤토리에 아이템 추가
        /// <para/> 넣는 데 실패한 잉여 아이템 개수 리턴
        /// <para/> 리턴이 0이면 넣는데 모두 성공했다는 의미
        /// </summary>
        public int Add(ItemData data, int count = 1)
        {
            int index;

            // 수량이 있는 아이템일 경우는 다음과 같다.
            if (data.IsCountable == true)
            {
                bool findNextCountable = true;
                index = -1;

                while (count > 0)
                {
                    // 이미 해당 아이템이 인벤토리 내에 존재하고, 개수 여유 있는지 검사한다.
                    if (findNextCountable)
                    {
                        index = FindCountableItemSlotIndex(data, index + 1);

                        // 개수가 여유있는 존재하는 슬롯이 더이상 없다고 판단될 경우, 빈 슬롯부터 탐색을 시작한다.
                        if (index == -1)
                        {
                            findNextCountable = false;
                        }
                        // 존재하는 슬롯을 찾은 경우, 양을 증가시키고 초과량 존재 시 수량을 초기화한다.
                        else
                        {
                            count = items[index].Sum(count);

                            UpdateSlot(index);
                        }
                    }
                    // 빈 슬롯을 탐색한다.
                    else
                    {
                        index = FindEmptySlotIndex(index + 1);

                        // 빈 슬롯조차 없는 경우 종료한다.
                        if (index == -1)
                        {
                            break;
                        }

                        // 빈 슬롯 발견 시, 슬롯에 아이템 추가 및 잉여량을 계산한다.
                        data.Count = count;

                        // 슬롯에 추가한다.
                        items[index] = data;

                        // 남은 개수 계산한다.
                        count = count > ItemData.MaximumStackCount ? count - ItemData.MaximumStackCount : 0;

                        UpdateSlot(index);
                    }
                }
            }
            // 수량이 없는 아이템일 경우는 다음과 같다.
            else
            {
                // 1개만 넣는 경우, 간단히 수행한다.
                if (count == 1)
                {
                    index = FindEmptySlotIndex();
                    if (index != -1)
                    {
                        // 아이템을 생성하여 슬롯에 추가한다.
                        items[index] = data;
                        count = 0;

                        UpdateSlot(index);
                    }
                }

                // 2개 이상의 수량 없는 아이템을 동시에 추가하는 경우는 다음과 같다.
                index = -1;
                for (; count > 0; count--)
                {
                    // 아이템 넣은 인덱스의 다음 인덱스부터 슬롯 탐색한다.
                    index = FindEmptySlotIndex(index + 1);

                    // 다 넣지 못한 경우에는 루프 종료한다.
                    if (index == -1)
                    {
                        break;
                    }

                    // 아이템을 생성하여 슬롯에 추가한다.
                    items[index] = data;

                    UpdateSlot(index);
                }
            }

            return count;
        }

        /// <summary>
        /// 해당 슬롯의 아이템 제거한다.
        /// </summary>
        public void Remove(int index)
        {
            if (IsValidIndex(index) == false)
            {
                return;
            }

            items[index] = null;
            inventoryView.RemoveItem(index);
        }

        /// <summary>
        /// 두 슬롯의 아이템 위치를 서로 교체한다.
        /// </summary>
        public void Swap(int a, int b)
        {
            if (IsValidIndex(a) == false)
            {
                return;
            }

            if (IsValidIndex(b) == false)
            {
                return;
            }

            (items[a], items[b]) = (items[b], items[a]);

            // 두 슬롯 정보 갱신한다.
            UpdateSlot(a, b);
        }

        /// <summary>
        /// 빈 슬롯 없이 앞에서부터 채운다.
        /// </summary>
        public void TrimAll()
        {
            // 가장 빠른 배열 빈공간 채우기 알고리즘

            // i 커서와 j 커서
            // i 커서 : 가장 앞에 있는 빈칸을 찾는 커서
            // j 커서 : i 커서 위치에서부터 뒤로 이동하며 기존재 아이템을 찾는 커서

            // i커서가 빈칸을 찾으면 j 커서는 i+1 위치부터 탐색
            // j커서가 아이템을 찾으면 아이템을 옮기고, i 커서는 i+1 위치로 이동
            // j커서가 Capacity에 도달하면 루프 즉시 종료

            _indexSetForUpdate.Clear();

            int i = -1;
            while (items[++i] != null)
            {
                ;
            }

            int j = i;

            while (true)
            {
                while (++j < Capacity && items[j] == null)
                {
                    ;
                }

                if (j == Capacity)
                {
                    break;
                }

                _indexSetForUpdate.Add(i);
                _indexSetForUpdate.Add(j);

                items[i] = items[j];
                items[j] = null;
                i++;
            }

            foreach (var index in _indexSetForUpdate)
            {
                UpdateSlot(index);
            }
        }
    }
}
