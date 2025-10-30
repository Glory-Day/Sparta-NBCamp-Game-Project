using Backend.Util.Presentation;
using TMPro;
using UnityEngine;

namespace Script.Object.UI
{
    public class TextView : MonoBehaviour, IView
    {
        [field: Header("Text References")]
        [field: SerializeField] public TMP_Text Text { get; private set; }

        public void Change(string text)
        {
            Text.text = text;
        }
    }
}
