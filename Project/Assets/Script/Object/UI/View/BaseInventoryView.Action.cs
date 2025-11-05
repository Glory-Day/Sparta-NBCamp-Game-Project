using System;
using System.Collections;
using System.Collections.Generic;
using Backend.Util.Debug;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Backend.Object.UI
{
    public partial class BaseInventoryView
    {
        protected Vector2 _cursorPos;

        protected ItemSlotView _selectedSlot;

        public Action<int> RemoveAction;
        public Action<int> InfoAction;

        protected virtual void PressLeftMouseButton(InputAction.CallbackContext context)
        {
            _pointerEventData.position = _cursorPos;
            _selectedSlot = Raycast<ItemSlotView>();

            Debugger.LogMessage($"LeftMouseButton Activated");
            if (_selectedSlot != null && _selectedSlot.HasItem)
            {
                Debugger.LogMessage($"Left MouseButton Used Selected : {_selectedSlot}");
                InfoAction?.Invoke(_selectedSlot.Index);
            }
        }

        private void OnMouseMove(InputAction.CallbackContext context)
        {
            _cursorPos = context.ReadValue<Vector2>();
            _pointerEventData.position = _cursorPos;

            if (_selectedSlot == null)
            {
                return;
            }
        }

        protected T Raycast<T>() where T : Component
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
    }
}
