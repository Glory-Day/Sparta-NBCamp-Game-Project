using System;
using System.Collections;
using System.Collections.Generic;
using Backend.Util.Presentation.Message;
using Script.Object.UI;
using UnityEngine;

namespace Backend.Object.UI.View
{
    public class StatusTextView : TextView
    {
        public Action StatusAction;
        public StatusType StatusType;
        public void OnEnable()
        {
            StatusAction?.Invoke();
        }
        public void OnDisable()
        {
            StatusAction?.Invoke();
        }
    }
}
