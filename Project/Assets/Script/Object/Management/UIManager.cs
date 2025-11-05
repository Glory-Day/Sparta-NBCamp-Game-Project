using System.Collections.Generic;
using System.Linq;
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
        private readonly Dictionary<string, IWindow> _defaults = new();
        private readonly Dictionary<string, IWindow> _caches = new();

        private Canvas _canvas;

        private UIControls _controls;

        protected override void OnAwake()
        {
            _canvas = GetComponentInChildren<Canvas>();
        }

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

        private void AddAsDefault_Internal(string key, IWindow value)
        {
            var origin = (value as Window)?.gameObject;
            var clone = ObjectPoolManager.SpawnPoolObject(origin, null, null, _canvas.transform);
            var window = clone.GetComponent<Window>();

            _defaults.Add(key, window);

            clone.SetActive(false);
        }

        private void Add_Internal(string key, IWindow value)
        {
            var origin = (value as Window)?.gameObject;
            var clone = ObjectPoolManager.SpawnPoolObject(origin, null, null, _canvas.transform);
            var window = clone.GetComponent<Window>();

            _caches.Add(key, window);

            clone.SetActive(false);
        }

        private void Remove_Internal(string key)
        {
            _caches.Remove(key);
        }

        private bool ContainKey_Internal(string key)
        {
            return _caches.ContainsKey(key);
        }

        private Window GetDefaultWindow_Internal(string key)
        {
            return _defaults.TryGetValue(key, out var value) == false ? null : value as Window;
        }

        private Window GetCachedWindow_Internal(string key)
        {
            return _caches.TryGetValue(key, out var value) == false ? null : value as Window;
        }

        private void Clear_Internal()
        {
            var windows = _caches.Values.ToArray();
            var length = windows.Length;
            for (var i = 0; i < length; i++)
            {
                windows[i].Close();
            }

            _caches.Clear();
        }

        private void CloseAllDefaultWindows_Internal()
        {
            var windows = _defaults.Values.ToArray();
            var length = windows.Length;
            for (var i = 0; i < length; i++)
            {
                windows[i].Close();
            }
        }

        private void ToggleStatusWindow(InputAction.CallbackContext context)
        {
            var isOpened = _caches[AddressData.Assets_Prefab_UI_Status_Window_Prefab].IsOpened;
            if (isOpened)
            {
                _caches[AddressData.Assets_Prefab_UI_Status_Window_Prefab].Close();
            }
            else
            {
                _caches[AddressData.Assets_Prefab_UI_Status_Window_Prefab].Open();
            }
        }

        private void ToggleLevelUpWindow(InputAction.CallbackContext context)
        {
            var isOpened = _caches[AddressData.Assets_Prefab_UI_Level_Up_Window_Prefab].IsOpened;
            if (isOpened)
            {
                _caches[AddressData.Assets_Prefab_UI_Level_Up_Window_Prefab].Close();
            }
            else
            {
                _caches[AddressData.Assets_Prefab_UI_Level_Up_Window_Prefab].Open();
            }
        }

        private void ToggleInventoryWindow(InputAction.CallbackContext context)
        {
            var isOpened = _caches[AddressData.Assets_Prefab_UI_Inventory_Window_Prefab].IsOpened;
            if (isOpened)
            {
                _caches[AddressData.Assets_Prefab_UI_Inventory_Window_Prefab].Close();
            }
            else
            {
                _caches[AddressData.Assets_Prefab_UI_Inventory_Window_Prefab].Open();
            }
        }

        private void ToggleEquipmentWindow(InputAction.CallbackContext context)
        {
            var isOpened = _caches[AddressData.Assets_Prefab_UI_Equipment_Window_Prefab].IsOpened;
            if (isOpened)
            {
                _caches[AddressData.Assets_Prefab_UI_Equipment_Window_Prefab].Close();
            }
            else
            {
                _caches[AddressData.Assets_Prefab_UI_Equipment_Window_Prefab].Open();
            }
        }

        private void ToggleShopWindow(InputAction.CallbackContext context)
        {
            var isOpened = _caches[AddressData.Assets_Prefab_UI_Shop_Window_Prefab].IsOpened;
            if (isOpened)
            {
                _caches[AddressData.Assets_Prefab_UI_Shop_Window_Prefab].Close();
            }
            else
            {
                _caches[AddressData.Assets_Prefab_UI_Shop_Window_Prefab].Open();
            }
        }

        private void ToggleSettingWindow(InputAction.CallbackContext context)
        {
            var isOpened = _defaults[AddressData.Assets_Prefab_UI_Setting_Window_Prefab].IsOpened;
            if (isOpened)
            {
                _defaults[AddressData.Assets_Prefab_UI_Setting_Window_Prefab].Close();
            }
            else
            {
                _defaults[AddressData.Assets_Prefab_UI_Setting_Window_Prefab].Open();
            }
        }

        public static void Add(string key, IWindow value)
        {
            Instance.Add_Internal(key, value);
        }

        public static void AddAsDefault(string key, IWindow value)
        {
            Instance.AddAsDefault_Internal(key, value);
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

        public static void CloseAllDefaultWindows()
        {
            Instance.CloseAllDefaultWindows_Internal();
        }

        public static Window GetDefaultWindow(string key)
        {
            return Instance.GetDefaultWindow_Internal(key);
        }

        public static Window GetCachedWindow(string key)
        {
            return Instance.GetCachedWindow_Internal(key);
        }
    }
}
