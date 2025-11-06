using System;
using System.Collections;
using System.Collections.Generic;
using Backend.Util.Management;
using Backend.Util.Presentation;
using UnityEngine;
using UnityEngine.UI;

namespace Backend.Object.UI.View
{
    public class SettingView : MonoBehaviour, IView
    {
        public Button CloseButton;
        public Action PauseAction;

        public void OnEnable()
        {
            PauseAction?.Invoke();
        }
    }
}
