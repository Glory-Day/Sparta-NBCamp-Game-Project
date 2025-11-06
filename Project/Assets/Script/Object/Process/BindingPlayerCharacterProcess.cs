using System;
using System.Collections;
using Backend.Object.Character.Player;
using Backend.Object.Management;
using Backend.Object.NPC;
using Backend.Object.UI;
using Backend.Util.Data;
using Script.Object.Character.Player;
using Script.Object.UI;
using UnityEngine;

namespace Backend.Object.Process
{
    [Serializable]
    public class BindingPlayerCharacterProcess : IProcessable
    {
        public IEnumerator Running()
        {
            var data = Data.PlayerCharacterData;
            var position = data[0].Position;
            var rotation = data[0].Rotation;

            var key = AddressData.Assets_Prefab_Character_Player_Player_Character_Beta_Prefab;
            var origin = Util.Management.ResourceManager.GetGameObjectAsset<GameObject>(key);
            var clone = ObjectPoolManager.SpawnPoolObject(origin, position, rotation, null);
            var model = clone.GetComponent<PlayerStatus>();

            key = AddressData.Assets_Prefab_UI_Battle_Interface_Window_Prefab;
            origin = UIManager.GetCachedWindow(key).gameObject;

            var binder01 = origin.GetComponentInChildren<PlayerConditionInformationBinder>();
            binder01.Bind(model);
            origin.SetActive(true);

            key = AddressData.Assets_Prefab_UI_Level_Up_Window_1_Prefab;
            origin = UIManager.GetCachedWindow(key).gameObject;

            var binder02 = origin.GetComponent<PlayerLevelStatusInformationBinder>();
            binder02.Bind(model);

            key = AddressData.Assets_Prefab_UI_Inventory_Window_1_Prefab;
            origin = UIManager.GetCachedWindow(key).gameObject;

            var binder03 = origin.GetComponent<PlayerInventoryInformationBinder>();
            binder03.Bind(new Inventory(32, 81), model);

            key = AddressData.Assets_Prefab_UI_Status_Window_1_Prefab;
            key = AddressData.Assets_Prefab_UI_Equipment_Window_1_Prefab;

            Target = clone;

            yield return null;
        }

        public SpawnData Data { get; set; }

        public GameObject Target { get; set; }
    }
}
