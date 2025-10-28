using System.Collections.Generic;
using Backend.Util.Data;
using Backend.Util.Presentation;
using UnityEngine;

namespace Backend.Object.UI
{
    public class Inventory : IModel //모델
    {
        /// <summary> 아이템 수용 한도 </summary>
        public int Capacity { get; private set; }

        // 초기 수용 한도
        [SerializeField, Range(8, 81)]
        private int _initalCapacity = 32;

        // 최대 수용 한도(아이템 배열 크기)
        [SerializeField, Range(8, 81)]
        private int _maxCapacity = 81;

        /// <summary> 아이템 목록 </summary>
        [SerializeField]
        public ItemData[] items;

        // 아이템 장착 여부 및 슬롯 번호
        private bool _isArmorEquip = false;
        private bool _isWeaponEquip = false;
        private int _currentArmorEquip;
        private int _currentWeaponEquip;

        //생성자 쓰기
        public Inventory(int capacity, int maxCapacity)
        {
            _maxCapacity = maxCapacity;
            Capacity = capacity;
            items = new ItemData[capacity];
        }
#if UNITY_EDITOR

        private void OnValidate()
        {
            if (_initalCapacity > _maxCapacity)
            {
                _initalCapacity = _maxCapacity;
            }
        }
#endif
    }
}
