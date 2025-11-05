using System;
using System.Collections;
using Backend.Object.Character.Player;
using Backend.Object.Management;
using Backend.Object.UI;
using Backend.Util.Data;
using Script.Object.UI;
using UnityEngine;

namespace Backend.Object.Process
{
    [Serializable]
    public class BindingPlayerCharacterProcess : IProcessable
    {
        public IEnumerator Running()
        {
            var currentSceneIndex = SceneManager.CurrentSceneIndex;
            var key = currentSceneIndex switch
            {
                1 => AddressData.Assets_Data_Spawn_Forest_01_Spawn_Data_Asset,
                2 => AddressData.Assets_Data_Spawn_Village_01_Spawn_Data_Asset,
                3 => AddressData.Assets_Data_Spawn_Boss_01_Spawn_Data_Asset,
                _ => string.Empty
            };

            var data = Util.Management.ResourceManager.GetDataAsset<SpawnData>(key).PlayerCharacterData;
            var position = data[0].Position;
            var rotation = data[0].Rotation;

            key = AddressData.Assets_Prefab_Character_Player_Player_Character_Beta_Prefab;
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

        public GameObject Target { get; set; }
    }
}
