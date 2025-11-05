using System;
using System.Buffers;
using Backend.Util.Debug;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Backend.Object.UI.View
{
    public class ItemInventoryView : BaseInventoryView
    {
        private ItemSlotView _pointerOverSlot;
        private Transform _selectedSlotTrans;

        private Vector3 _selectedSlotPos;
        private Vector2 _beginDragCursorPoint;
        private int _selectedSlotSiblingIndex;

        public Action<int> EquipAction;
        public Action<int, int> SwapAction;

        protected void OnEnable()
        {
            base.OnEnable();
            _controls.Inventory.Drag.performed += Drag;
            _controls.Inventory.ClickLeftMouseButton.canceled += ReleaseLeftMouseButton;
        }

        protected void OnDisable()
        {
            _controls.Inventory.Drag.performed -= Drag;
            _controls.Inventory.ClickLeftMouseButton.canceled -= ReleaseLeftMouseButton;
            base.OnDisable();
        }

        protected sealed override void PressLeftMouseButton(InputAction.CallbackContext context)
        {
            base.PressLeftMouseButton(context);
            if (_selectedSlot != null && _selectedSlot.HasItem)
            {
                Debugger.LogMessage($"EmptySlot Select. You Can Choose Item. Slot : {_selectedSlot}");

                _selectedSlotTrans = _selectedSlot.IconImageRectTransform.transform;
                _selectedSlotPos = _selectedSlotTrans.position;
                _beginDragCursorPoint = _cursorPos;

                _selectedSlotSiblingIndex = _selectedSlot.transform.GetSiblingIndex();
                _selectedSlot.transform.SetAsLastSibling();

                _selectedSlot.HighlightImageRectTransform.SetAsFirstSibling();
                EquipAction.Invoke(_selectedSlot.Index);
            }
            else
            {
                _selectedSlot = null;
            }
        }

        private void ReleaseLeftMouseButton(InputAction.CallbackContext context)
        {
            if(_selectedSlot == null)
            {
                return;
            }

            _selectedSlotTrans.position = _selectedSlotPos;
            _selectedSlot.transform.SetSiblingIndex(_selectedSlotSiblingIndex);

            DragCompleted();

            _selectedSlot.HighlightImageRectTransform.SetAsLastSibling();

            _selectedSlot = null;
            _selectedSlotTrans = null;

        }

        private void Drag(InputAction.CallbackContext context)
        {
            _cursorPos = context.ReadValue<Vector2>();
            _pointerEventData.position = _cursorPos;

            if (_selectedSlot == null)
            {
                return;
            }

            // 위치를 이동시킨다.
            var delta = _cursorPos - _beginDragCursorPoint;
            _selectedSlotTrans.position = _selectedSlotPos + (Vector3)delta;
        }

        private void DragCompleted()
        {
            var draggedSlot = Raycast<ItemSlotView>();

            // 아이템 슬롯끼리 아이템을 교환하거나 이동한다.
            if (draggedSlot != null)
            {
                Swap(_selectedSlot, draggedSlot);

                return;
            }

            // 커서가 레이캐스트 타겟 위에 있지 않은 경우에는 버린다.
            if (IsPointerOverGameObject() != false)
            {
                return;
            }

            // 확인 팝업 띄우고 콜백을 위임한다.
            var index = _selectedSlot.Index;
            //var itemName = _inventory.GetItemName(index);
            //var count = _inventory.GetCurrentAmount(index);

            //// 셀 수 있는 아이템의 경우에 수량을 표시한다.
            //if (count > 1)
            //{
            //    itemName += $"{count}";
            //}

            //if (_showRemovingPopup)
            //{
            //    _popup.OpenConfirmationPopup(() => Remove(index), itemName);
            //}
            //else
            //{
            //    Remove(index);
            //}
        }

        private void Swap(ItemSlotView a, ItemSlotView b)
        {
            if (a == b)
            {
                return;
            }

            a.Move(b);

            SwapAction?.Invoke(a.Index, b.Index);
        }

        private static bool IsPointerOverGameObject()
        {
            return EventSystem.current.IsPointerOverGameObject();
        }
    }
}
