using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Backend.Object.UI
{
    public class InventoryUI : MonoBehaviour
    {
        [Header("Options")]
        [Range(0, 10)]
        [SerializeField] private int _horizontalSlotCount = 8;  // 슬롯 가로 개수
        [Range(0, 10)]
        [SerializeField] private int _verticalSlotCount = 8;      // 슬롯 세로 개수
        [SerializeField] private float _slotMargin = 8f;          // 한 슬롯의 상하좌우 여백
        [SerializeField] private float _contentAreaPadding = 20f; // 인벤토리 영역의 내부 여백
        [Range(32, 64)]
        [SerializeField] private float _slotSize = 64f;      // 각 슬롯의 크기

        [Header("Connected Objects")]
        [SerializeField] private RectTransform _contentAreaRT; // 슬롯들이 위치할 영역
        [SerializeField] private GameObject _slotUiPrefab;     // 슬롯의 원본 프리팹

        //private Inventory _inventory;
        private List<ItemSlotUI> _slotUIList = new List<ItemSlotUI>();

        private GraphicRaycaster _gr;
        private PointerEventData _ped;
        private List<RaycastResult> _rrList;

        private ItemSlotUI _beginDragSlot; // 현재 드래그를 시작한 슬롯
        private Transform _beginDragIconTransform; // 해당 슬롯의 아이콘 트랜스폼

        private Vector3 _beginDragIconPoint;   // 드래그 시작 시 슬롯의 위치
        private Vector3 _beginDragCursorPoint; // 드래그 시작 시 커서의 위치
        private int _beginDragSlotSiblingIndex;

        private Inventory _inventory;

        private void Awake()
        {
            InitSlots();
        }

        private void Update()
        {
            //_ped.position = Input.mousePosition;dddddddddddddddddddddddddddd

            OnPointerDown();
            OnPointerDrag();
            OnPointerUp();
        }

        public void SetInventoryReference(Inventory inventory)
        {
            _inventory = inventory;
        }

        /// <summary> 지정된 개수만큼 슬롯 영역 내에 슬롯들 동적 생성 </summary>
        private void InitSlots()
        {
            // 슬롯 프리팹 설정
            _slotUiPrefab.TryGetComponent(out RectTransform slotRect);
            slotRect.sizeDelta = new Vector2(_slotSize, _slotSize);

            _slotUiPrefab.TryGetComponent(out ItemSlotUI itemSlot);
            if (itemSlot == null)
                _slotUiPrefab.AddComponent<ItemSlotUI>();

            _slotUiPrefab.SetActive(false);

            // --
            Vector2 beginPos = new Vector2(_contentAreaPadding, -_contentAreaPadding);
            Vector2 curPos = beginPos;

            _slotUIList = new List<ItemSlotUI>(_verticalSlotCount * _horizontalSlotCount);

            // 슬롯들 동적 생성
            for (int j = 0; j < _verticalSlotCount; j++)
            {
                for (int i = 0; i < _horizontalSlotCount; i++)
                {
                    int slotIndex = (_horizontalSlotCount * j) + i;

                    var slotRT = CloneSlot();
                    slotRT.pivot = new Vector2(0f, 1f); // Left Top
                    slotRT.anchoredPosition = curPos;
                    slotRT.gameObject.SetActive(true);
                    slotRT.gameObject.name = $"Item Slot [{slotIndex}]";

                    var slotUI = slotRT.GetComponent<ItemSlotUI>();
                    slotUI.SetSlotIndex(slotIndex);
                    _slotUIList.Add(slotUI);

                    // Next X
                    curPos.x += (_slotMargin + _slotSize);
                }

                // Next Line
                curPos.x = beginPos.x;
                curPos.y -= (_slotMargin + _slotSize);
            }

            // 슬롯 프리팹 - 프리팹이 아닌 경우 파괴
            if (_slotUiPrefab.scene.rootCount != 0)
                Destroy(_slotUiPrefab);

            // -- Local Method --
            RectTransform CloneSlot()
            {
                GameObject slotGo = Instantiate(_slotUiPrefab);
                RectTransform rt = slotGo.GetComponent<RectTransform>();
                rt.SetParent(_contentAreaRT);
                slotGo.transform.localScale = Vector3.one;

                return rt;
            }
        }

        private T RaycastAndGetFirstComponent<T>() where T : Component
        {
            _rrList.Clear();

            _gr.Raycast(_ped, _rrList);

            if (_rrList.Count == 0)
                return null;

            return _rrList[0].gameObject.GetComponent<T>();
        }

        private void OnPointerDown()
        {
            // Left Click : Begin Drag
            if (Input.GetMouseButtonDown(0))
            {
                _beginDragSlot = RaycastAndGetFirstComponent<ItemSlotUI>();

                // 아이템을 갖고 있는 슬롯만 해당
                if (_beginDragSlot != null && _beginDragSlot.HasItem)
                {
                    // 위치 기억, 참조 등록
                    _beginDragIconTransform = _beginDragSlot.IconRect.transform;
                    _beginDragIconPoint = _beginDragIconTransform.position;
                    _beginDragCursorPoint = Input.mousePosition;

                    // 맨 위에 보이기
                    _beginDragSlotSiblingIndex = _beginDragSlot.transform.GetSiblingIndex();
                    _beginDragSlot.transform.SetAsLastSibling();

                    // 해당 슬롯의 하이라이트 이미지를 아이콘보다 뒤에 위치시키기
                    //_beginDragSlot.SetHighlightOnTop(false);dddddddddddddddddddddddd
                }
                else
                {
                    _beginDragSlot = null;
                }
            }
        }

        /// <summary> 드래그하는 도중 </summary>
        private void OnPointerDrag()
        {
            if (_beginDragSlot == null)
                return;

            if (Input.GetMouseButton(0))
            {
                // 위치 이동
                _beginDragIconTransform.position =
                    _beginDragIconPoint + (Input.mousePosition - _beginDragCursorPoint);
            }
        }
        /// <summary> 클릭을 뗄 경우 </summary>
        private void OnPointerUp()
        {
            if (Input.GetMouseButtonUp(0))
            {
                // End Drag
                if (_beginDragSlot != null)
                {
                    // 위치 복원
                    _beginDragIconTransform.position = _beginDragIconPoint;

                    // UI 순서 복원
                    _beginDragSlot.transform.SetSiblingIndex(_beginDragSlotSiblingIndex);

                    // 드래그 완료 처리
                    //EndDrag(); dddddddddddddddddddd

                    // 참조 제거
                    _beginDragSlot = null;
                    _beginDragIconTransform = null;
                }
            }
        }

        //private void EndDrag()
        //{
        //    ItemSlotUI endDragSlot = RaycastAndGetFirstComponent<ItemSlotUI>();

        //    // 아이템 슬롯끼리 아이콘 교환 또는 이동
        //    if (endDragSlot != null && endDragSlot.IsAccessible)
        //    {
        //        TrySwapItems(_beginDragSlot, endDragSlot);
        //    }

        //    // 버리기(커서가 UI 레이캐스트 타겟 위에 있지 않은 경우)
        //    if (!IsOverUI())
        //    {
        //        //TryRemoveItem(index);
        //    }
        //}

        //private void TryRemoveItem(int index)
        //{
        //    _inventory.Remove(index);
        //}

        private bool IsOverUI()
            => EventSystem.current.IsPointerOverGameObject();

        /// <summary> 두 슬롯의 아이템 교환 </summary>
        //private void TrySwapItems(ItemSlotUI from, ItemSlotUI to)
        //{
        //    if (from == to)
        //    {
        //        return;
        //    }

        //    from.SwapOrMoveIcon(to);
        //    _inventory.Swap(from.Index, to.Index);
        //}

#if UNITY_EDITOR
        [SerializeField] private bool __showPreview = false;

        [Range(0.01f, 1f)]
        [SerializeField] private float __previewAlpha = 0.1f;

        private List<GameObject> __previewSlotGoList = new List<GameObject>();
        private int __prevSlotCountPerLine;
        private int __prevSlotLineCount;
        private float __prevSlotSize;
        private float __prevSlotMargin;
        private float __prevContentPadding;
        private float __prevAlpha;
        private bool __prevShow = false;
        private bool __prevMouseReversed = false;

        private void OnValidate()
        {
            //if (__prevMouseReversed != _mouseReversed)
            //{
            //    __prevMouseReversed = _mouseReversed;
            //    InvertMouse(_mouseReversed);

            //    EditorLog($"Mouse Reversed : {_mouseReversed}");
            //}dddddddddddddddddddddddddddddddddd

            if (Application.isPlaying)
                return;

            if (__showPreview && !__prevShow)
            {
                CreateSlots();
            }
            __prevShow = __showPreview;

            if (Unavailable())
            {
                ClearAll();
                return;
            }
            if (CountChanged())
            {
                ClearAll();
                CreateSlots();
                __prevSlotCountPerLine = _horizontalSlotCount;
                __prevSlotLineCount = _verticalSlotCount;
            }
            if (ValueChanged())
            {
                DrawGrid();
                __prevSlotSize = _slotSize;
                __prevSlotMargin = _slotMargin;
                __prevContentPadding = _contentAreaPadding;
            }
            if (AlphaChanged())
            {
                SetImageAlpha();
                __prevAlpha = __previewAlpha;
            }

            bool Unavailable()
            {
                return !__showPreview ||
                        _horizontalSlotCount < 1 ||
                        _verticalSlotCount < 1 ||
                        _slotSize <= 0f ||
                        _contentAreaRT == null ||
                        _slotUiPrefab == null;
            }
            bool CountChanged()
            {
                return _horizontalSlotCount != __prevSlotCountPerLine ||
                        _verticalSlotCount != __prevSlotLineCount;
            }
            bool ValueChanged()
            {
                return _slotSize != __prevSlotSize ||
                        _slotMargin != __prevSlotMargin ||
                        _contentAreaPadding != __prevContentPadding;
            }
            bool AlphaChanged()
            {
                return __previewAlpha != __prevAlpha;
            }
            void ClearAll()
            {
                foreach (var go in __previewSlotGoList)
                {
                    Destroyer.Destroy(go);
                }
                __previewSlotGoList.Clear();
            }
            void CreateSlots()
            {
                int count = _horizontalSlotCount * _verticalSlotCount;
                __previewSlotGoList.Capacity = count;

                // 슬롯의 피벗은 Left Top으로 고정
                RectTransform slotPrefabRT = _slotUiPrefab.GetComponent<RectTransform>();
                slotPrefabRT.pivot = new Vector2(0f, 1f);

                for (int i = 0; i < count; i++)
                {
                    GameObject slotGo = Instantiate(_slotUiPrefab);
                    slotGo.transform.SetParent(_contentAreaRT.transform);
                    slotGo.SetActive(true);
                    slotGo.AddComponent<PreviewItemSlot>();

                    slotGo.transform.localScale = Vector3.one; // 버그 해결

                    HideGameObject(slotGo);

                    __previewSlotGoList.Add(slotGo);
                }

                DrawGrid();
                SetImageAlpha();
            }
            void DrawGrid()
            {
                Vector2 beginPos = new Vector2(_contentAreaPadding, -_contentAreaPadding);
                Vector2 curPos = beginPos;

                // Draw Slots
                int index = 0;
                for (int j = 0; j < _verticalSlotCount; j++)
                {
                    for (int i = 0; i < _horizontalSlotCount; i++)
                    {
                        GameObject slotGo = __previewSlotGoList[index++];
                        RectTransform slotRT = slotGo.GetComponent<RectTransform>();

                        slotRT.anchoredPosition = curPos;
                        slotRT.sizeDelta = new Vector2(_slotSize, _slotSize);
                        __previewSlotGoList.Add(slotGo);

                        // Next X
                        curPos.x += (_slotMargin + _slotSize);
                    }

                    // Next Line
                    curPos.x = beginPos.x;
                    curPos.y -= (_slotMargin + _slotSize);
                }
            }
            void HideGameObject(GameObject go)
            {
                go.hideFlags = HideFlags.HideAndDontSave;

                Transform tr = go.transform;
                for (int i = 0; i < tr.childCount; i++)
                {
                    tr.GetChild(i).gameObject.hideFlags = HideFlags.HideAndDontSave;
                }
            }
            void SetImageAlpha()
            {
                foreach (var go in __previewSlotGoList)
                {
                    var images = go.GetComponentsInChildren<Image>();
                    foreach (var img in images)
                    {
                        img.color = new Color(img.color.r, img.color.g, img.color.b, __previewAlpha);
                        var outline = img.GetComponent<Outline>();
                        if (outline)
                            outline.effectColor = new Color(outline.effectColor.r, outline.effectColor.g, outline.effectColor.b, __previewAlpha);
                    }
                }
            }
        }

        private class PreviewItemSlot : MonoBehaviour { }

        [UnityEditor.InitializeOnLoad]
        private static class Destroyer
        {
            private static Queue<GameObject> targetQueue = new Queue<GameObject>();

            static Destroyer()
            {
                UnityEditor.EditorApplication.update += () =>
                {
                    for (int i = 0; targetQueue.Count > 0 && i < 100000; i++)
                    {
                        var next = targetQueue.Dequeue();
                        DestroyImmediate(next);
                    }
                };
            }
            public static void Destroy(GameObject go) => targetQueue.Enqueue(go);
        }
#endif
    }
}
