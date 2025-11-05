using System;
using System.Collections;
using System.Collections.Generic;
using Backend.Util.Presentation;
using Script.Object.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Backend.Object.UI.View
{
    public class WeaponCalculateView : MonoBehaviour, IView
    {
        public TextMeshProUGUI[] text;

        public Action CaculateAction;

        public void Change(int index, string cal)
        {
            text[index].text = cal;
        }

        private void OnEnable()
        {
            CaculateAction?.Invoke();
        }
    }
}
