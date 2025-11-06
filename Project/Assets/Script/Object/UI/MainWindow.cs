using UnityEngine;
using UnityEngine.UI;

namespace Backend.Object.UI
{
    public class MainWindow : Window
    {
        [field: Header("UI References")]
        [field: SerializeField] public Button StartButton { get; private set; }
        [field: SerializeField] public Button LoadButton { get; private set; }

        public void SetLoadButtonInteractable(bool value)
        {
            LoadButton.interactable = value;
        }
    }
}
