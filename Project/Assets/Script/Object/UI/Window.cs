using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Backend.Object.UI
{
    public class Window : MonoBehaviour, IWindow
    {
        bool IWindow.IsOpened => gameObject.activeSelf;

        public void Close()
        {
            gameObject.SetActive(false);
        }
        public void Open()
        {
            gameObject.SetActive(true);
        }
    }
}
