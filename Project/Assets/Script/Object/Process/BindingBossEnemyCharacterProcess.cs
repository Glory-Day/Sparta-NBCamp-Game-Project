using System.Collections;
using Backend.Object.Character.Enemy;
using Backend.Object.Management;
using Backend.Util.Data;
using Script.Object.UI;
using UnityEngine;

namespace Backend.Object.Process
{
    public class BindingBossEnemyCharacterProcess : IProcessable
    {
        public IEnumerator Running()
        {
            var currentSceneIndex = SceneManager.CurrentSceneIndex;
            var key = currentSceneIndex switch
            {
                3 => AddressData.Assets_Data_Spawn_Boss_01_Spawn_Data_Asset,
                _ => string.Empty
            };

            if (key == string.Empty)
            {
                yield break;
            }

            var asset = Util.Management.ResourceManager.GetDataAsset<SpawnData>(key);
            var data = asset.BossEnemyCharacterData[0];

            var position = data.Position;
            var rotation = data.Rotation;

            key = currentSceneIndex switch
            {
                3 => AddressData.Assets_Prefab_Character_Enemy_Boss_NineTail_Human_NineTail_Human_Prefab,
                _ => string.Empty
            };

            var origin = Util.Management.ResourceManager.GetGameObjectAsset<GameObject>(key);
            var clone = ObjectPoolManager.SpawnPoolObject(origin, position, rotation, null);
            var model = clone.GetComponent<EnemyStatus>();
            var controller = clone.GetComponent<EnemyMovementController>();
            controller.Target = Target;

            key = AddressData.Assets_Prefab_UI_Battle_Interface_Window_Prefab;
            origin = UIManager.GetCachedWindow(key).gameObject;

            var binder = origin.GetComponentInChildren<BossEnemyConditionInformationBinder>();
            binder.Bind(model);
        }

        public GameObject Target { get; set; }
    }
}
