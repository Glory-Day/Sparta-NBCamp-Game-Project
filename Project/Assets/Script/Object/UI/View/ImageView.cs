using System.Collections;
using System.Collections.Generic;
using Backend.Util.Presentation;
using UnityEngine;
using UnityEngine.UI;

namespace Backend.Object.UI.View
{
    public class ImageView : MonoBehaviour, IView
    {
        [Header("UI References")]
        [SerializeField] protected Image Img;

        public virtual void Change(Sprite img)
        {
            Img.sprite = img;
        }
    }
}
