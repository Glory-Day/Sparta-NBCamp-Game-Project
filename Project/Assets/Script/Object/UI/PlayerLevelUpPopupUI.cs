using System;
using System.Collections;
using System.Collections.Generic;
using Backend.Object.Character.Player;
using Backend.Util.Data;
using Backend.Util.Debug;
using Backend.Util.Presentation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Backend.Object.UI
{
    public class PlayerLevelUpPopupUI : MonoBehaviour, IView
    {
        [Header("Amount Input Popup")]
        [SerializeField] private GameObject amountInputPopupObject;
        [SerializeField] private TextMeshProUGUI amountText;
        [SerializeField] private Button amountPlusButton;
        [SerializeField] private Button amountMinusButton;
        public Button amountInputOkButton;
        [SerializeField] private Button amountInputCancelButton;

        public int Point { get; private set; }
        public int StatusIndex { get; private set; }

        private void Awake()
        {
            amountText.text = Point.ToString();
            InitUIEvents();
        }

        public void Open(int statusIndex)
        {
            StatusIndex = statusIndex;
            amountText.text = "0";
            amountInputPopupObject.SetActive(true);
        }


        private void InitUIEvents()
        {
            amountInputCancelButton.onClick.AddListener(() =>
            {
                amountInputPopupObject.SetActive(false);
                Point = 0;
            });

            // - 
            amountMinusButton.onClick.AddListener(() =>
            {
                if(Point > 0)
                {
                    Point--;
                }
                amountText.text = Point.ToString();
            });

            // +
            amountPlusButton.onClick.AddListener(() =>
            {
                if(Point < 99)
                {
                    Point++;
                }
                amountText.text = Point.ToString();
            });
        }
    }
}
