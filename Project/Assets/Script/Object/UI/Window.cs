using System;
using Backend.Util.Debug;
using UnityEngine;

namespace Backend.Object.UI
{
    public class Window : MonoBehaviour, IWindow
    {
        private RectTransform _rectTransform;

        private bool _isFullStretched;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();

            // Check this window object is full stretched mode.
            if (_rectTransform.anchorMin == Vector2.zero && _rectTransform.anchorMax == Vector2.one)
            {
                _isFullStretched = true;
            }
        }

        public void OnEnable()
        {
            if (_isFullStretched == false)
            {
                return;
            }

            _rectTransform.offsetMin = Vector2.zero;
            _rectTransform.offsetMax = Vector2.zero;
        }

        public void Open()
        {
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        bool IWindow.IsOpened => gameObject.activeSelf;
    }
}
