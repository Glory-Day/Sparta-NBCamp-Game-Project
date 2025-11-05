using System.Collections;
using System.Collections.Generic;
using Backend.Util.Presentation;
using Backend.Util.Presentation.Message;
using UnityEngine;

namespace Backend.Object.UI.View
{
    public class ActiveView : MonoBehaviour, IView
    {
        [SerializeField] private GameObject view;
        public LayoutType layoutType;

        public virtual void Change(bool value)
        {
            view.SetActive(value);
        }
    }
}
