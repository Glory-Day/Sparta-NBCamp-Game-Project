using System.Collections;
using Backend.Util.Presentation;
using UnityEngine;
using UnityEngine.UI;

namespace Backend.Object.UI.View
{
    public class PointBarView : MonoBehaviour, IView
    {
        [Header("Data Information")]
        [Range(0f, 1f)]
        [SerializeField] protected float percentage = 1f;

        [Header("UI References")]
        [SerializeField] protected Image foregroundImage;

        private float _cache;

        protected virtual void Awake()
        {
            foregroundImage.fillAmount = percentage;
            _cache = percentage;
        }

        public virtual void Change(float value)
        {
            _cache = foregroundImage.fillAmount;
            foregroundImage.fillAmount = value;
        }
    }
}
