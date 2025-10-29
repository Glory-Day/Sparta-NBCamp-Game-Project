using System.Collections.Generic;
using Backend.Object.UI;
using Backend.Util.Input;
using Backend.Util.Management;
using UnityEngine.InputSystem;

namespace Script.Object.Management
{
    public class UIManager : SingletonGameObject<UIManager>
    {
        public enum WindowType
        {
            Inventory
        }

        private UIControls _controls;
        private readonly Dictionary<WindowType, IWindow> _windows = new();
        private void OnEnable()
        {
            _controls = new UIControls();
            _controls.Enable();
            _controls.Main.OpenInventory.performed += ToggleInventoryWindow;
        }

        private void OnDisable()
        {
            _controls.Main.OpenInventory.performed -= ToggleInventoryWindow;
            _controls.Disable();
            _controls = null;
        }

        private void AddWindow_Internal(WindowType windowType, IWindow window)
        {
            _windows.Add(windowType, window);
        }

        private void ToggleInventoryWindow(InputAction.CallbackContext context)
        {
            var isOpened = _windows[WindowType.Inventory].IsOpened;
            if (isOpened)
            {
                _windows[WindowType.Inventory].Close();
            }
            else
            {
                _windows[WindowType.Inventory].Open();
            }
        }
    }
}
