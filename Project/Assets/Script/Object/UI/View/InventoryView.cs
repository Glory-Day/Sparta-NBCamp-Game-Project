using System.Collections.Generic;
using Backend.Util.Debug;
using Backend.Util.Input;
using Backend.Util.Presentation;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Test.Data.Base;
using Test.Data;

/*
    [기능 - 에디터 전용]
    - 게임 시작 시 동적으로 생성될 슬롯 미리보기(개수, 크기 미리보기 가능)

    [기능 - 유저 인터페이스]
    - 슬롯에 마우스 올리기
      - 사용 가능 슬롯 : 하이라이트 이미지 표시
      - 아이템 존재 슬롯 : 아이템 정보 툴팁 표시

    - 드래그 앤 드롭
      - 아이템 존재 슬롯 -> 아이템 존재 슬롯 : 두 아이템 위치 교환
      - 아이템 존재 슬롯 -> 아이템 미존재 슬롯 : 아이템 위치 변경
        - Shift 또는 Ctrl 누른 상태일 경우 : 셀 수 있는 아이템 수량 나누기
      - 아이템 존재 슬롯 -> UI 바깥 : 아이템 버리기

    - 슬롯 우클릭
      - 사용 가능한 아이템일 경우 : 아이템 사용

    - 기능 버튼(좌측 상단)
      - Trim : 앞에서부터 빈 칸 없이 아이템 채우기
      - Sort : 정해진 가중치대로 아이템 정렬

    - 필터 버튼(우측 상단)
      - [A] : 모든 아이템 필터링
      - [E] : 장비 아이템 필터링
      - [P] : 소비 아이템 필터링

      * 필터링에서 제외된 아이템 슬롯들은 조작 불가

    [기능 - 기타]
    - InvertMouse(bool) : 마우스 좌클릭/우클릭 반전 여부 설정
*/

namespace Backend.Object.UI
{
    public partial class InventoryView : MonoBehaviour
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

        [Space]
        [SerializeField] private bool _showTooltip = true;
        [SerializeField] private bool _showHighlight = true;
        [SerializeField] private bool _showRemovingPopup = true;

        [Header("Connected Objects")]
        [SerializeField] private RectTransform _contentAreaRT;             // 슬롯들이 위치할 영역
        [SerializeField] private GameObject _slotUiPrefab;                 // 슬롯의 원본 프리팹
        [SerializeField] private ItemInformationView _itemInformationView; // 아이템 정보를 보여줄 툴팁 UI
        [SerializeField] private InventoryPopupUI _popup;                  // 팝업 UI 관리 객체

        [Header("Buttons")]
        [SerializeField] private Button _trimButton;
        [SerializeField] private Button _sortButton;

        [Header("Filter Toggles")]
        [SerializeField] private Toggle _toggleFilterAll;
        [SerializeField] private Toggle _toggleFilterEquipments;
        [SerializeField] private Toggle _toggleFilterPortions;

        [Space(16)]
        [SerializeField] private bool _mouseReversed = false; // 마우스 클릭 반전 여부

        #endregion

        /// <summary> 연결된 인벤토리 </summary>
        private Inventory _inventory;

        private List<ItemSlotView> _slots = new ();

        private GraphicRaycaster _graphicRaycaster;
        private List<RaycastResult> _raycastResults;

        private PointerEventData _pointerEventData;

        private UIControls.InventoryActions _controls;

        /// <summary>
        /// 인벤토리 UI 내 아이템 종류에 대한 필터링 옵션.
        /// </summary>
        private enum Filter
        {
            All, Equipment, Portion
        }
        private Filter _filter = Filter.All;

        private void Awake()
        {
            _controls = new UIControls().Inventory;

            TryGetComponent(out _graphicRaycaster);
            if (_graphicRaycaster == null)
            {
                _graphicRaycaster = gameObject.AddComponent<GraphicRaycaster>();
            }

            _pointerEventData = new PointerEventData(EventSystem.current);
            _raycastResults = new List<RaycastResult>(10);

            if (_itemInformationView == null)
            {
                _itemInformationView = GetComponentInChildren<ItemInformationView>();

                if (_showDebug)
                {
                    Debugger.LogMessage("인스펙터에서 아이템 정보 창 오브젝트 레퍼런스를 직접 지정하지 않아 자식에서 발견하여 초기화.");
                }
            }

            // 아이템 슬롯 뷰 프리팹을 설정한다.
            _slotUiPrefab.TryGetComponent<RectTransform>(out var rectTransform);
            rectTransform.sizeDelta = new Vector2(_slotSize, _slotSize);

            _slotUiPrefab.TryGetComponent<ItemSlotView>(out var itemSlotView);
            if (itemSlotView == null)
            {
                _slotUiPrefab.AddComponent<ItemSlotView>();
            }

            _slotUiPrefab.SetActive(false);

            var beginPos = new Vector2(_contentAreaPadding, -_contentAreaPadding);
            var anchoredPosition = beginPos;

            _slots = new List<ItemSlotView>(_verticalSlotCount * _horizontalSlotCount);

            // 슬롯 들 동적 생성
            for (var i = 0; i < _verticalSlotCount; i++)
            {
                for (var j = 0; j < _horizontalSlotCount; j++)
                {
                    var index = (_horizontalSlotCount * i) + j;

                    // 생성한 슬롯 인스턴스의 피벗을 좌상단으로 설정한다.
                    var clone = Clone();
                    clone.pivot = new Vector2(0f, 1f);
                    clone.anchoredPosition = anchoredPosition;
                    clone.gameObject.SetActive(true);
                    clone.gameObject.name = $"Item Slot [{index}]";

                    var slot = clone.GetComponent<ItemSlotView>();
                    slot.Index = index;
                    _slots.Add(slot);

                    // Next X
                    anchoredPosition.x += _slotMargin + _slotSize;
                }

                // Next Line
                anchoredPosition.x = beginPos.x;
                anchoredPosition.y -= _slotMargin + _slotSize;
            }

            // 슬롯 프리팹 - 프리팹이 아닌 경우 파괴
            if (_slotUiPrefab.scene.rootCount != 0)
            {
                Destroy(_slotUiPrefab);
            }

            _trimButton.onClick.AddListener(() => _inventory.TrimAll());

            _toggleFilterAll.onValueChanged.AddListener(flag => UpdateFilter(flag, Filter.All));
            _toggleFilterEquipments.onValueChanged.AddListener(flag => UpdateFilter(flag, Filter.Equipment));
            _toggleFilterPortions.onValueChanged.AddListener(flag => UpdateFilter(flag, Filter.Portion));
        }

        private void OnEnable()
        {
            _controls.Enable();
            _controls.Drag.performed += Drag;
            _controls.ClickLeftMouseButton.performed += PressLeftMouseButton;
            _controls.ClickLeftMouseButton.canceled += ReleaseLeftMouseButton;
            _controls.ClickRightMouseButton.performed += PressRightMouseButton;
            _controls.PressLeftControlButton.performed += PressLeftControlButton;
            _controls.PressLeftControlButton.canceled += ReleaseLeftControlButton;
            _controls.PressLeftShiftButton.performed += PressLeftShiftButton;
            _controls.PressLeftShiftButton.canceled += ReleaseLeftShiftButton;
        }

        private void Update()
        {
            OnPointerEnterAndExit();

            if (_showTooltip)
            {
                ShowOrHideItemTooltip();
            }
        }

        private void OnDisable()
        {
            _controls.Drag.performed -= Drag;
            _controls.ClickLeftMouseButton.performed -= PressLeftMouseButton;
            _controls.ClickLeftMouseButton.canceled -= ReleaseLeftMouseButton;
            _controls.ClickRightMouseButton.performed -= PressRightMouseButton;
            _controls.PressLeftControlButton.performed -= PressLeftControlButton;
            _controls.PressLeftControlButton.canceled -= ReleaseLeftControlButton;
            _controls.PressLeftShiftButton.performed -= PressLeftShiftButton;
            _controls.PressLeftShiftButton.canceled -= ReleaseLeftShiftButton;
            _controls.Disable();
        }

        private RectTransform Clone()
        {
            //TODO: Object Pool Manager를 사용한 인스턴스 생성 방법으로 교체해야 한다.
            var rectTransform = Instantiate(_slotUiPrefab).GetComponent<RectTransform>();;
            rectTransform.SetParent(_contentAreaRT);
            rectTransform.transform.localScale = Vector3.one;

            return rectTransform;
        }

        private void UpdateFilter(bool flag, Filter option)
        {
            if (flag == false)
            {
                return;
            }

            _filter = option;

            UpdateAllFilters();
        }

        /// <summary> 인벤토리 참조 등록 (인벤토리에서 직접 호출) </summary>
        public void SetInventoryReference(Inventory inventory)
        {
            _inventory = inventory;
        }

        /// <summary> 슬롯에 아이템 아이콘 등록 </summary>
        public void SetItemIconImageByIndex(int index, Sprite icon)
        {
            if (_showDebug)
            {
                Debugger.LogMessage($"Set Item Icon : Slot [{index}]");
            }

            _slots[index].ItemImage = icon;
        }

        /// <summary> 해당 슬롯의 아이템 개수 텍스트 지정 </summary>
        public void SetItemAmountTextByIndex(int index, int amount)
        {
            if (_showDebug)
            {
                Debugger.LogMessage($"Set Item Amount Text : Slot [{index}], Amount [{amount}]");
            }

            // NOTE : amount가 1 이하일 경우 텍스트 미표시
            _slots[index].Amount = amount;
        }

        /// <summary> 해당 슬롯의 아이템 개수 텍스트 지정 </summary>
        public void HideItemAmountText(int index)
        {
            if (_showDebug)
            {
                Debugger.LogMessage($"Hide Item Amount Text : Slot [{index}]");
            }

            _slots[index].Amount = 1;
        }

        /// <summary>
        ///해당 슬롯의 아이템 장착 여부 텍스트 지정
        /// </summary>
        /// <param name="index">슬롯 번호</param>
        /// <param name="equip">장착 여부</param>
        public void SetEquipmentText(int index, bool equip)
        {
            //테스트중
            _slots[index].IsEquipped = equip;
        }

        /// <summary> 슬롯에서 아이템 아이콘 제거, 개수 텍스트 숨기기 </summary>
        public void RemoveItem(int index)
        {
            if (_showDebug)
            {
                Debugger.LogMessage($"Remove Item : Slot [{index}]");
            }

            _slots[index].Remove();
        }

        /// <summary> 접근 가능한 슬롯 범위 설정 </summary>
        public void SetAccessibleSlotRange(int accessibleSlotCount)
        {
            for (int i = 0; i < _slots.Count; i++)
            {
                _slots[i].IsSlotAccessible = i < accessibleSlotCount;
            }
        }

        /// <summary> 특정 슬롯의 필터 상태 업데이트 </summary>
        public void UpdateFilter(int index, ItemData data)
        {
            bool isFiltered = true;

            // null인 슬롯은 타입 검사 없이 필터 활성화
            if (data != null)
            {
                isFiltered = _filter switch
                {
                    Filter.Equipment => data is EquipmentItemData,
                    Filter.Portion => data is PortionItemData,
                    _ => true
                };
            }

            _slots[index].IsItemAccessible = isFiltered;
        }

        /// <summary> 모든 슬롯 필터 상태 업데이트 </summary>
        public void UpdateAllFilters()
        {
            int capacity = _inventory.Capacity;

            for (int i = 0; i < capacity; i++)
            {
                var data = _inventory.GetItemData(i);

                UpdateFilter(i, data);
            }
        }
    }
}
