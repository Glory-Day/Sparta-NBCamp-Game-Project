using Backend.Util.Presentation;
using TMPro;
using UnityEngine;

namespace Script.Object.UI.View
{
    public class PointTextView : MonoBehaviour, IView
    {
        [field: Header("Text References")]
        [field: SerializeField] public TMP_Text CurrentPointText { get; private set; }

        public void Change(int point)
        {
            CurrentPointText.text = point.ToString() + " >";
        }
    }
}
