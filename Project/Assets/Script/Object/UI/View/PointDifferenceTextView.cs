using System;
using TMPro;
using UnityEngine;

namespace Script.Object.UI.View
{
    public class PointDifferenceTextView : PointTextView
    {
        [field: SerializeField] public TMP_Text UpdatedPointText { get; private set; }

        [field: Header("Display Type")]
        [field: SerializeField] private DisplayType type;

        [field: SerializeField] private Color increasedColor;
        [field: SerializeField] private Color decreasedColor;

        public void Change(string current, string updated)
        {
            base.Change(current);

            UpdatedPointText.text = updated;

            switch (type)
            {
                case DisplayType.Default:
                    break;
                case DisplayType.Color:
                {
                    var a = int.Parse(CurrentPointText.text);
                    var b = int.Parse(UpdatedPointText.text);
                    var difference = b - a;

                    UpdatedPointText.color = difference switch
                    {
                        > 0 => increasedColor,
                        < 0 => decreasedColor,
                        _ => Color.white
                    };

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #region NESTED ENUMERATION API

        private enum DisplayType
        {
            Default,
            Color
        }

        #endregion
    }
}
