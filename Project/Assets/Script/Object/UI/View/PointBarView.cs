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

        protected virtual void Awake()
        {
            foregroundImage.fillAmount = percentage;
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            foregroundImage.fillAmount = percentage;
        }

#endif

        public virtual void Change(float value)
        {
            percentage = value;
            foregroundImage.fillAmount = percentage;
        }

        public float Percentage => percentage;
    }
}
