using System.Collections;
using Backend.Util.Debug;
using UnityEngine;
using UnityEngine.UI;

namespace Backend.Object.UI.View
{
    public class DelayedPointBarView : PointBarView
    {
        [SerializeField] private Image backgroundImage;

        protected override void Awake()
        {
            base.Awake();

            backgroundImage.fillAmount = percentage;
        }

        public override void Change(float value)
        {
            StopAllCoroutines();

            base.Change(value);

            if (value < backgroundImage.fillAmount)
            {
                StartCoroutine(Changing());
            }
            else
            {
                backgroundImage.fillAmount = value;
            }
        }

        private IEnumerator Changing()
        {
            var a = foregroundImage.fillAmount;
            var b = backgroundImage.fillAmount;

            for (var i = 0f; i <= 1f; i += Time.deltaTime)
            {
                backgroundImage.fillAmount = Mathf.Lerp(b, a, i);

                yield return null;
            }
        }
    }
}
