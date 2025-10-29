using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Backend.Object.UI
{
    public interface IWindow
    {
        void Open();
        void Close();
        bool IsOpened { get; }
    }
}
