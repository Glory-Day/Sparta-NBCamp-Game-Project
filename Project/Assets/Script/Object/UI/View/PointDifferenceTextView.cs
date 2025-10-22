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

        public void Change(int current, int updated)
        {
            base.Change(current);

            UpdatedPointText.text = updated.ToString();

            switch (type)
            {
                case DisplayType.Default:
                    break;
                case DisplayType.Color:
                {
                    var difference = updated - current;

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
