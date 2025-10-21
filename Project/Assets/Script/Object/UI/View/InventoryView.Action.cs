using Backend.Util.Debug;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Backend.Object.UI
{
    public partial class InventoryView
    {
        private bool _isLeftControlButtonPressed;
        private bool _isLeftShiftButtonPressed;

        private Vector2 _cursorPosition;

        private ItemSlotView _pointerOverSlot;     // 현재 포인터가 위치한 곳의 슬롯
        private ItemSlotView _selectedSlot;       // 현재 드래그를 시작한 슬롯
        private Transform _selectedSlotTransform; // 해당 슬롯의 아이콘 트랜스폼

        private Vector3 _selectedSlotPosition;   // 드래그 시작 시 슬롯의 위치
        private Vector2 _beginDragCursorPoint; // 드래그 시작 시 커서의 위치
        private int _selectedSlotSiblingIndex;

        /// <summary>
        /// 슬롯에 포인터가 올라가는 경우, 슬롯에서 포인터가 빠져나가는 경우
        /// </summary>
        private void OnPointerEnterAndExit()
        {
            // 이전 프레임의 슬롯
            var previous = _pointerOverSlot;

            // 현재 프레임의 슬롯
            var current = _pointerOverSlot = Raycast<ItemSlotView>();

            if (previous == null)
            {
                // Enter
                if (current == null)
                {
                    return;
                }

                if (_showHighlight)
                {
                    current.IsHighlighted = true;
                }
            }
            else
            {

                if (current == null)
                {
                    // Exit
                    previous.IsHighlighted = false;
                }
                else if (previous != current)
                {
                    // Change
                    previous.IsHighlighted = false;
                    if (_showHighlight)
                    {
                        current.IsHighlighted = true;
                    }
                }
            }
        }

        /// <summary>
        /// 아이템 정보 툴팁 보여주거나 감추기
        /// </summary>
        private void ShowOrHideItemTooltip()
        {
            if (_pointerOverSlot == null)
            {
                _itemInformationView.Hide();

                return;
            }

            // 마우스가 유효한 아이템 아이콘 위에 올라와 있다면 툴팁 보여주기
            var hasItem = _pointerOverSlot.HasItem;
            var isAccessible = _pointerOverSlot.IsAccessible;
            var isDragged = _pointerOverSlot != _selectedSlot;
            var isValid = hasItem && isAccessible && isDragged;

            if (isValid)
            {
                UpdateItemInventoryView(_pointerOverSlot);

                _itemInformationView.Show();
            }
            else
            {
                _itemInformationView.Hide();
            }
        }

        private void PressLeftMouseButton(InputAction.CallbackContext context)
        {
            _pointerEventData.position = _cursorPosition;

            _selectedSlot = Raycast<ItemSlotView>();

            if (_selectedSlot != null && _selectedSlot.HasItem && _selectedSlot.IsAccessible)
            {
                _selectedSlotTransform = _selectedSlot.IconImageRectTransform.transform;
                _selectedSlotPosition = _selectedSlotTransform.position;
                _beginDragCursorPoint = _cursorPosition;
                _selectedSlotSiblingIndex = _selectedSlot.transform.GetSiblingIndex();
                _selectedSlot.transform.SetAsLastSibling();
                _selectedSlot.HighlightImageRectTransform.SetAsFirstSibling();
            }
            else
            {
                _selectedSlot = null;
            }
        }

        private void PressRightMouseButton(InputAction.CallbackContext context)
        {
            _pointerEventData.position = _cursorPosition;

            var slot = Raycast<ItemSlotView>();
            if (slot != null && slot.HasItem && slot.IsAccessible)
            {
                TryUseItem(slot.Index);
            }
        }

        private void ReleaseLeftMouseButton(InputAction.CallbackContext context)
        {
            if (_selectedSlot == null)
            {
                return;
            }

            _selectedSlotTransform.position = _selectedSlotPosition;
            _selectedSlot.transform.SetSiblingIndex(_selectedSlotSiblingIndex);
            EndDrag();
            _selectedSlot.HighlightImageRectTransform.SetAsLastSibling();
            _selectedSlot = null;
            _selectedSlotTransform = null;
        }

        /// <summary> 슬롯에 클릭하는 경우 </summary>
        private void OnPointerDown()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                // Left Click : Begin Drag
                _selectedSlot = Raycast<ItemSlotView>();

                // 아이템을 갖고 있는 슬롯만 해당
                if (_selectedSlot != null && _selectedSlot.HasItem && _selectedSlot.IsAccessible)
                {
                    if (_showDebug)
                    {
                        Debugger.LogMessage($"Drag Begin : Slot [{_selectedSlot.Index}]");
                    }

                    // 위치 기억, 참조 등록
                    _selectedSlotTransform = _selectedSlot.IconImageRectTransform.transform;
                    _selectedSlotPosition = _selectedSlotTransform.position;
                    _beginDragCursorPoint = Mouse.current.position.ReadValue();

                    // 맨 위에 보이기
                    _selectedSlotSiblingIndex = _selectedSlot.transform.GetSiblingIndex();
                    _selectedSlot.transform.SetAsLastSibling();

                    // 해당 슬롯의 하이라이트 이미지를 아이콘보다 뒤에 위치시키기
                    _selectedSlot.HighlightImageRectTransform.SetAsFirstSibling();
                }
                else
                {
                    _selectedSlot = null;
                }
            }
            else if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                // Right Click : Use Item
                var slot = Raycast<ItemSlotView>();
                if (slot != null && slot.HasItem && slot.IsAccessible)
                {
                    TryUseItem(slot.Index);
                }
            }
        }

        private void Drag(InputAction.CallbackContext context)
        {
            _cursorPosition = context.ReadValue<Vector2>();
            _pointerEventData.position = _cursorPosition;

            if (_selectedSlot == null)
            {
                return;
            }

            var delta = _cursorPosition - _beginDragCursorPoint;
            _selectedSlotTransform.position = _selectedSlotPosition + (Vector3)delta;
        }

        private void PressLeftControlButton(InputAction.CallbackContext context)
        {
            _isLeftControlButtonPressed = true;
        }

        private void ReleaseLeftControlButton(InputAction.CallbackContext context)
        {
            _isLeftControlButtonPressed = false;
        }

        private void PressLeftShiftButton(InputAction.CallbackContext context)
        {
            _isLeftShiftButtonPressed = true;
        }

        private void ReleaseLeftShiftButton(InputAction.CallbackContext context)
        {
            _isLeftShiftButtonPressed = false;
        }

        /// <summary> 드래그하는 도중 </summary>
        private void OnPointerDrag()
        {
            if (_selectedSlot == null)
            {
                return;
            }

            if (Mouse.current.leftButton.isPressed)
            {
                // 위치 이동
                var delta = Mouse.current.position.ReadValue() - _beginDragCursorPoint;
                _selectedSlotTransform.position = _selectedSlotPosition + (Vector3)delta;
            }
        }

        /// <summary> 클릭을 뗄 경우 </summary>
        private void OnPointerUp()
        {
            if (Mouse.current.leftButton.wasReleasedThisFrame == false)
            {
                return;
            }

            // End Drag
            if (_selectedSlot == null)
            {
                return;
            }

            // 위치 복원
            _selectedSlotTransform.position = _selectedSlotPosition;

            // UI 순서 복원
            _selectedSlot.transform.SetSiblingIndex(_selectedSlotSiblingIndex);

            // 드래그 완료 처리
            EndDrag();

            // 해당 슬롯의 하이라이트 이미지를 아이콘보다 앞에 위치시키기
            _selectedSlot.HighlightImageRectTransform.SetAsLastSibling();

            // 참조 제거
            _selectedSlot = null;
            _selectedSlotTransform = null;
        }

        private void EndDrag()
        {
            ItemSlotView endDragSlot = Raycast<ItemSlotView>();

            // 아이템 슬롯끼리 아이콘 교환 또는 이동
            if (endDragSlot != null && endDragSlot.IsAccessible)
            {
                // 수량 나누기 조건
                // 1) 마우스 클릭 떼는 순간 좌측 Ctrl 또는 Shift 키 유지
                // 2) begin : 셀 수 있는 아이템 / end : 비어있는 슬롯
                // 3) begin 아이템의 수량 > 1
                bool isSeparable =
                    (_isLeftControlButtonPressed || _isLeftShiftButtonPressed) &&
                    _inventory.IsCountableItem(_selectedSlot.Index) &&
                    !_inventory.HasItem(endDragSlot.Index);


                // true : 수량 나누기, false : 교환 또는 이동
                bool isSeparation = false;
                int currentAmount = 0;

                // 현재 개수 확인
                if (isSeparable)
                {
                    currentAmount = _inventory.GetCurrentAmount(_selectedSlot.Index);
                    if (currentAmount > 1)
                    {
                        isSeparation = true;
                    }
                }

                if (isSeparation)
                {
                    // 1. 개수 나누기
                    TrySeparateAmount(_selectedSlot.Index, endDragSlot.Index, currentAmount);
                }
                else
                {
                    // 2. 교환 또는 이동
                    TrySwapItems(_selectedSlot, endDragSlot);
                }

                // 툴팁 갱신
                UpdateItemInventoryView(endDragSlot);

                return;
            }

            // 버리기(커서가 UI 레이캐스트 타겟 위에 있지 않은 경우)
            if (!IsPointerOverGameObject())
            {
                // 확인 팝업 띄우고 콜백 위임
                var index = _selectedSlot.Index;
                var itemName = _inventory.GetItemName(index);
                var amount = _inventory.GetCurrentAmount(index);

                // 셀 수 있는 아이템의 경우, 수량 표시
                if (amount > 1)
                {
                    itemName += $" x{amount}";
                }

                if (_showRemovingPopup)
                {
                    _popup.OpenConfirmationPopup(() => TryRemoveItem(index), itemName);
                }
                else
                {
                    TryRemoveItem(index);
                }
            }
            // 슬롯이 아닌 다른 UI 위에 놓은 경우
            else
            {
                if (_showDebug)
                {
                    Debugger.LogMessage("Drag End(Do Nothing)");
                }
            }
        }

        /// <summary>
        /// 레이캐스트하여 얻은 첫 번째 UI에서 컴포넌트 찾아서 반환한다.
        /// </summary>
        private T Raycast<T>() where T : Component
        {
            _raycastResults.Clear();
            _graphicRaycaster.Raycast(_pointerEventData, _raycastResults);

            if (_raycastResults.Count == 0)
            {
                return null;
            }

            var instance = _raycastResults[0].gameObject;
            var component = instance.GetComponent<T>();

            return component;
        }

        /// <summary> UI 및 인벤토리에서 아이템 제거 </summary>
        private void TryRemoveItem(int index)
        {
            if (_showDebug)
            {
                Debugger.LogMessage($"UI - Try Remove Item : Slot [{index}]");
            }

            _inventory.Remove(index);
        }

        /// <summary> 아이템 사용 </summary>
        private void TryUseItem(int index)
        {
            if (_showDebug)
            {
                Debugger.LogMessage($"UI - Try Use Item : Slot [{index}]");
            }

            _inventory.Use(index);
        }

        /// <summary> 두 슬롯의 아이템 교환 </summary>
        private void TrySwapItems(ItemSlotView from, ItemSlotView to)
        {
            if (from == to)
            {
                if (_showDebug)
                {
                    Debugger.LogMessage($"UI - Try Swap Items: Same Slot [{from.Index}]");
                }

                return;
            }

            if (_showDebug)
            {
                Debugger.LogMessage($"UI - Try Swap Items: Slot [{from.Index} -> {to.Index}]");
            }

            from.Move(to);
            _inventory.Swap(from.Index, to.Index);
        }

        /// <summary> 셀 수 있는 아이템 개수 나누기 </summary>
        private void TrySeparateAmount(int indexA, int indexB, int amount)
        {
            if (indexA == indexB)
            {
                if (_showDebug)
                {
                    Debugger.LogMessage($"UI - Try Separate Amount: Same Slot [{indexA}]");
                }

                return;
            }

            if (_showDebug)
            {
                Debugger.LogMessage($"UI - Try Separate Amount: Slot [{indexA} -> {indexB}]");
            }

            string itemName = $"{_inventory.GetItemName(indexA)} x{amount}";

            _popup.OpenAmountInputPopup(
                amt => _inventory.SeparateAmount(indexA, indexB, amt),
                amount, itemName
            );
        }

        /// <summary> 툴팁 UI의 슬롯 데이터 갱신 </summary>
        private void UpdateItemInventoryView(ItemSlotView slot)
        {
            if (slot.IsAccessible == false || slot.HasItem == false)
            {
                return;
            }

            // 툴팁 정보 갱신
            _itemInformationView.Data = _inventory.GetItemData(slot.Index);

            // 툴팁 위치 조정
            _itemInformationView.SetRectPosition(slot.SlotImageRectTransform);
        }

        private static bool IsPointerOverGameObject()
        {
            return EventSystem.current.IsPointerOverGameObject();
        }
    }
}
