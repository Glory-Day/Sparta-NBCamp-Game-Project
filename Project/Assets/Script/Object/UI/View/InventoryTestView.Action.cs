using System;
using System.Collections;
using System.Collections.Generic;
using Backend.Util.Debug;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Backend.Object.UI
{
    public partial class InventoryTestView
    {
        private Vector2 _cursorPos;

        private ItemSlotView _selectedSlot;

        public Action<int> RemoveAction;
        public Action<int> InfoAction;
        public Action UpdateAction;

        protected void PressLeftMouseButton(InputAction.CallbackContext context)
        {
            _pointerEventData.position = _cursorPos;
            _selectedSlot = Raycast<ItemSlotView>();

            if(_selectedSlot != null && _selectedSlot.HasItem)
            {
                Debugger.LogMessage($"LefT MouseButton Used Selected : {_selectedSlot}");
                InfoAction?.Invoke(_selectedSlot.Index);
            }
        }

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
    }
}
