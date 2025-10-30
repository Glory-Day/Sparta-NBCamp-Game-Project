using System.Collections.Generic;
using Backend.Object.UI;
using Backend.Util.Data;
using Backend.Util.Input;
using Backend.Util.Management;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Backend.Object.Management
{
    public class UIManager : SingletonGameObject<UIManager>
    {
        private readonly Dictionary<string, IWindow> _windows = new();

        private UIControls _controls;

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

        private void Add_Internal(string key, IWindow value)
        {
            var origin = (value as Window)?.gameObject;
            var window = ObjectPoolManager.SpawnPoolObject(origin, null, null, transform).GetComponent<Window>();

            _windows.Add(key, window);
        }

        private void Remove_Internal(string key)
        {
            _windows.Remove(key);
        }

        private bool ContainKey_Internal(string key)
        {
            return _windows.ContainsKey(key);
        }

        private GameObject GetValue_Internal(string key)
        {
            return _windows.TryGetValue(key, out var value) == false ? null : (value as Window)?.gameObject;
        }

        private void Clear_Internal()
        {
            _windows.Clear();
        }

        private void ToggleStatusWindow(InputAction.CallbackContext context)
        {
            var isOpened = _windows[AddressData.Assets_Prefab_UI_Status_Window_Prefab].IsOpened;
            if (isOpened)
            {
                _windows[AddressData.Assets_Prefab_UI_Status_Window_Prefab].Close();
            }
            else
            {
                _windows[AddressData.Assets_Prefab_UI_Status_Window_Prefab].Open();
            }
        }

        private void ToggleLevelUpWindow(InputAction.CallbackContext context)
        {
            var isOpened = _windows[AddressData.Assets_Prefab_UI_Level_Up_Window_Prefab].IsOpened;
            if (isOpened)
            {
                _windows[AddressData.Assets_Prefab_UI_Level_Up_Window_Prefab].Close();
            }
            else
            {
                _windows[AddressData.Assets_Prefab_UI_Level_Up_Window_Prefab].Open();
            }
        }

        private void ToggleInventoryWindow(InputAction.CallbackContext context)
        {
            var isOpened = _windows[AddressData.Assets_Prefab_UI_Inventory_Window_Prefab].IsOpened;
            if (isOpened)
            {
                _windows[AddressData.Assets_Prefab_UI_Inventory_Window_Prefab].Close();
            }
            else
            {
                _windows[AddressData.Assets_Prefab_UI_Inventory_Window_Prefab].Open();
            }
        }

        private void ToggleEquipmentWindow(InputAction.CallbackContext context)
        {
            var isOpened = _windows[AddressData.Assets_Prefab_UI_Equipment_Window_Prefab].IsOpened;
            if (isOpened)
            {
                _windows[AddressData.Assets_Prefab_UI_Equipment_Window_Prefab].Close();
            }
            else
            {
                _windows[AddressData.Assets_Prefab_UI_Equipment_Window_Prefab].Open();
            }
        }

        private void ToggleShopWindow(InputAction.CallbackContext context)
        {
            var isOpened = _windows[AddressData.Assets_Prefab_UI_Shop_Window_Prefab].IsOpened;
            if (isOpened)
            {
                _windows[AddressData.Assets_Prefab_UI_Shop_Window_Prefab].Close();
            }
            else
            {
                _windows[AddressData.Assets_Prefab_UI_Shop_Window_Prefab].Open();
            }
        }

        private void ToggleSettingWindow(InputAction.CallbackContext context)
        {
            var isOpened = _windows[AddressData.Assets_Prefab_UI_Setting_Window_Prefab].IsOpened;
            if (isOpened)
            {
                _windows[AddressData.Assets_Prefab_UI_Setting_Window_Prefab].Close();
            }
            else
            {
                _windows[AddressData.Assets_Prefab_UI_Setting_Window_Prefab].Open();
            }
        }

        public static void Add(string key, IWindow value)
        {
            Instance.Add_Internal(key, value);
        }

        public static void Remove(string key)
        {
            Instance.Remove_Internal(key);
        }

        public static bool ContainKey(string key)
        {
            return Instance.ContainKey_Internal(key);
        }

        public static void Clear()
        {
            Instance.Clear_Internal();
        }

        public static GameObject GetValue(string key)
        {
            return Instance.GetValue_Internal(key);
        }
    }
}
