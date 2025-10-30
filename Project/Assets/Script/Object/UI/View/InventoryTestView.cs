using System.Collections.Generic;
using Backend.Object.Management;
using Backend.Util.Input;
using Backend.Util.Presentation;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Backend.Object.UI
{
    public partial class InventoryTestView : MonoBehaviour, IView
    {
        #region SERIALIZABLE FIELD API

        [Header("Layout Settings")]
        [Range(0, 10)]
        [SerializeField] private int _horizontalSlotCount = 8;  // 슬롯 가로 개수
        [Range(0, 10)]
        [SerializeField] private int _verticalSlotCount = 8;      // 슬롯 세로 개수
        [SerializeField] private float _slotMargin = 8f;          // 한 슬롯의 상하좌우 여백
        [SerializeField] private float _contentAreaPadding = 20f; // 인벤토리 영역의 내부 여백
        [Range(32, 64)]
        [SerializeField] private float _slotSize = 64f;      // 각 슬롯의 크기

        [Header("Connected Objects")]
        [SerializeField] private RectTransform _contentAreaRT;             // 슬롯들이 위치할 영역
        [SerializeField] private GameObject _slotUiPrefab;                 // 슬롯의 원본 프리팹

        #endregion

        public List<ItemSlotView> Slots = new();

        private GraphicRaycaster _graphicRaycaster;
        private List<RaycastResult> _raycastResults;

        private PointerEventData _pointerEventData;

        private UIControls _controls;

        private void OnEnable()
        {
            _controls = new UIControls();
            _controls.Inventory.Enable();
            _controls.Inventory.ClickLeftMouseButton.performed += PressLeftMouseButton;
        }

        private void OnDisable()
        {
            _controls.Inventory.Disable();
            _controls.Inventory.ClickLeftMouseButton.performed -= PressLeftMouseButton;
            _controls = null;
        }

        private void Awake()
        {
            TryGetComponent(out _graphicRaycaster);
            if(_graphicRaycaster == null)
            {
                _graphicRaycaster = gameObject.AddComponent<GraphicRaycaster>();
            }

            _pointerEventData = new PointerEventData(EventSystem.current);
            _raycastResults = new List<RaycastResult>(10);

            //슬롯당 크기 지정
            _slotUiPrefab.TryGetComponent<RectTransform>(out var rectTransform);
            rectTransform.sizeDelta = new Vector2(_slotSize, _slotSize);

            _slotUiPrefab.TryGetComponent<ItemSlotView>(out var itemSlotView);
            if(itemSlotView == null)
            {
                _slotUiPrefab.AddComponent<ItemSlotView>();
            }
            _slotUiPrefab.SetActive(false);

            var slotbeginPos = new Vector2(_contentAreaPadding, -_contentAreaPadding);
            var anchoredPos = slotbeginPos;

            Slots = new List<ItemSlotView>(_verticalSlotCount * _horizontalSlotCount);

            CreateSlot(anchoredPos, slotbeginPos);
        }

        private void CreateSlot(Vector2 anchoredPos, Vector2 slotBeginPos)
        {
            // 슬롯들 동적 생성
            for (var i = 0; i < _verticalSlotCount; i++)
            {
                for (var j = 0; j < _horizontalSlotCount; j++)
                {
                    var index = (_horizontalSlotCount * i) + j;

                    // 생성한 슬롯 인스턴스의 피벗을 좌상단으로 설정한다.
                    var clone = Clone();
                    clone.pivot = new Vector2(0f, 1f);
                    clone.anchoredPosition = anchoredPos;
                    clone.gameObject.SetActive(true);
                    clone.gameObject.name = $"Item Slot [{index}]";

                    var slot = clone.GetComponent<ItemSlotView>();
                    slot.Index = index;
                    Slots.Add(slot);

                    // Next X
                    anchoredPos.x += _slotMargin + _slotSize;
                }

                // Next Line
                anchoredPos.x = slotBeginPos.x;
                anchoredPos.y -= _slotMargin + _slotSize;
            }

            // 슬롯 프리팹 - 프리팹이 아닌 경우 파괴
            if (_slotUiPrefab.scene.rootCount != 0)
            {
                Destroy(_slotUiPrefab);
            }
        }

        private RectTransform Clone()
        {
            var instance = ObjectPoolManager.SpawnPoolObject(_slotUiPrefab, null, null, _contentAreaRT).GetComponent<RectTransform>();
            instance.transform.localScale = Vector3.one;

            return instance;
        }

        public void SetItemIconImageByIndex(int index, Sprite icon)
        {
            Slots[index].IconImage = icon;
        }

        /// <summary> 해당 슬롯의 아이템 개수 텍스트 지정 </summary>
        public void SetItemAmountTextByIndex(int index, int amount)
        {
            // NOTE : amount가 1 이하일 경우 텍스트 미표시
            Slots[index].Count = amount;
        }

        /// <summary> 해당 슬롯의 아이템 개수 텍스트 지정 </summary>
        public void HideItemAmountText(int index)
        {
            Slots[index].Count = 1;
        }

        /// <summary>
        ///해당 슬롯의 아이템 장착 여부 텍스트 지정
        /// </summary>
        /// <param name="index">슬롯 번호</param>
        /// <param name="equip">장착 여부</param>
        public void SetEquipmentText(int index, bool equip)
        {
            //테스트중
            Slots[index].IsEquipped = equip;
        }

        /// <summary> 슬롯에서 아이템 아이콘 제거, 개수 텍스트 숨기기 </summary>
        public void RemoveItem(int index)
        {
            Slots[index].Remove();
        }
    }
}
