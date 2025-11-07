using System;
using System.Collections;
using System.Collections.Generic;
using Backend.Object.UI.Presenter;
using Script.Object.UI.View;
using UnityEngine;

namespace Backend.Object.UI.View
{
    public class StatusDifferenceTextView : PointDifferenceTextView
    {
        public Action UpdateState;
        public void OnEnable()
        {
            UpdateState?.Invoke();
        }
    }
}
