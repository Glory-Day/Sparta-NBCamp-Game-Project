using System;
using Backend.Util.Debug;
using Backend.Util.Presentation;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Backend.Object.UI
{
    public partial class InventoryView : MonoBehaviour, IView
    {
        private Vector2 _cursorPosition;

        private ItemSlotView _pointerOverSlot;     // 현재 포인터가 위치한 곳의 슬롯
        private ItemSlotView _selectedSlot;       // 현재 드래그를 시작한 슬롯
        private Transform _selectedSlotTransform; // 해당 슬롯의 아이콘 트랜스폼

        private Vector3 _selectedSlotPosition;   // 드래그 시작 시 슬롯의 위치
        private Vector2 _beginDragCursorPoint; // 드래그 시작 시 커서의 위치
        private int _selectedSlotSiblingIndex;

        public Action<int> removeAction;
        public Action<int, int> swapAction;

        public Action<int> InfoAction;

        private void PressLeftMouseButton(InputAction.CallbackContext context)
        {
            _pointerEventData.position = _cursorPosition;

            _selectedSlot = Raycast<ItemSlotView>();

            // 아이템을 가지고 있는 슬롯인 경우에 다음과 같다.
            if (_selectedSlot != null && _selectedSlot.HasItem)
            {
                Debugger.LogProgress("Use LeftMouseButton Function Inner");

                // 위치 및 슬롯 관련 값를 저장한다.
                _selectedSlotTransform = _selectedSlot.IconImageRectTransform.transform;
                _selectedSlotPosition = _selectedSlotTransform.position;
                _beginDragCursorPoint = _cursorPosition;

                // 맨 위로 순서를 변경한다.
                _selectedSlotSiblingIndex = _selectedSlot.transform.GetSiblingIndex();
                _selectedSlot.transform.SetAsLastSibling();

                // 해당 슬롯의 하이라이트 이미지를 아이콘 이미지보다 뒤에 위치시킨다.
                _selectedSlot.HighlightImageRectTransform.SetAsFirstSibling();

                InfoAction?.Invoke(_selectedSlot.Index);
            }
            else
            {
                _selectedSlot = null;
            }
        }

        private void ReleaseLeftMouseButton(InputAction.CallbackContext context)
        {
            if (_selectedSlot == null)
            {
                return;
            }

            // 위치 및 순서를 복원시킨다.
            _selectedSlotTransform.position = _selectedSlotPosition;
            _selectedSlot.transform.SetSiblingIndex(_selectedSlotSiblingIndex);

            // 드래그를 완료 처리 시킨다.
            DragCompleted();

            // 해당 슬롯의 하이라이트 이미지를 아이콘 이미지보다 앞에 위치시킨다.
            _selectedSlot.HighlightImageRectTransform.SetAsLastSibling();

            // 참조된 슬롯관련 값을 제거한다.
            _selectedSlot = null;
            _selectedSlotTransform = null;
        }

        private void Drag(InputAction.CallbackContext context)
        {
            _cursorPosition = context.ReadValue<Vector2>();
            _pointerEventData.position = _cursorPosition;

            if (_selectedSlot == null)
            {
                return;
            }

            // 위치를 이동시킨다.
            var delta = _cursorPosition - _beginDragCursorPoint;
            _selectedSlotTransform.position = _selectedSlotPosition + (Vector3)delta;
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

        /// <summary>
        /// 인벤토리에서 아이템을 제거한다.
        /// </summary>
        private void Remove(int index)
        {
            removeAction?.Invoke(index);
        }

        /// <summary>
        /// 두 슬롯의 아이템을 교환한다.
        /// </summary>
        private void Swap(ItemSlotView a, ItemSlotView b)
        {
            if (a == b)
            {
                return;
            }

            a.Move(b);

            swapAction?.Invoke(a.Index, b.Index);
        }

        private static bool IsPointerOverGameObject()
        {
            return EventSystem.current.IsPointerOverGameObject();
        }
    }
}
