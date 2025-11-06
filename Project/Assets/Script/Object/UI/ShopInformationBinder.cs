using System.Collections;
using System.Collections.Generic;
using Backend.Object.Character.Player;
using Backend.Util.Presentation;
using UnityEngine;

namespace Backend.Object.UI
{
    public class ShopInformationBinder : MonoBehaviour
    {
        private Dispatcher _dispatcher = new();

        private void Init()
        {

        }

        public void Bind(Inventory[] inventory, PlayerStatus playerStatus)
        {
            Init();


        }
    }
}
