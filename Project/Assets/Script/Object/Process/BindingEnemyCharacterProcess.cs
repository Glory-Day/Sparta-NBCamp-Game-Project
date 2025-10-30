using Backend.Object.Character.Enemy;
using Backend.Object.Management;
using Backend.Util.Data;
using UnityEngine;

namespace Backend.Object.Process
{
    public class BindingEnemyCharacterProcess : IProcessable
    {
        public void Run()
        {
            var currentSceneIndex = SceneManager.CurrentSceneIndex;
            var key = currentSceneIndex switch
            {
                1 => AddressData.Assets_Data_Spawn_Forest_01_Spawn_Data_Asset,
                2 => AddressData.Assets_Data_Spawn_Village_01_Spawn_Data_Asset,
                _ => string.Empty
            };

            if (key == string.Empty)
            {
                return;
            }

            var asset = Util.Management.ResourceManager.GetDataAsset<SpawnData>(key);

            key = AddressData.Assets_Prefab_Character_Enemy_Normal_Skeleton_Sword_Prefab;
            var origin = Util.Management.ResourceManager.GetGameObjectAsset<GameObject>(key);

            var data = asset.WarriorEnemyCharacterData;
            var count = data.Count;
            for (var i = 0; i < count; i++)
            {
                var position = data[i].Position;
                var rotation = data[i].Rotation;

                var clone = ObjectPoolManager.SpawnPoolObject(origin, position, rotation, null);
                var model = clone.GetComponent<EnemyStatus>();
            }

            key = AddressData.Assets_Prefab_Character_Enemy_Normal_Skeleton_Bow_Prefab;
            origin = Util.Management.ResourceManager.GetGameObjectAsset<GameObject>(key);

            data = asset.ArcherEnemyCharacterData;
            count = data.Count;
            for (var i = 0; i < count; i++)
            {
                var position = data[i].Position;
                var rotation = data[i].Rotation;

                var clone = ObjectPoolManager.SpawnPoolObject(origin, position, rotation, null);
                var model = clone.GetComponent<EnemyStatus>();
            }

            key = AddressData.Assets_Prefab_Character_Enemy_Normal_Tiger_Prefab;
            origin = Util.Management.ResourceManager.GetGameObjectAsset<GameObject>(key);

            data = asset.MiniBossEnemyCharacterData;
            count = data.Count;
            for (var i = 0; i < count; i++)
            {
                var position = data[i].Position;
                var rotation = data[i].Rotation;

                var clone = ObjectPoolManager.SpawnPoolObject(origin, position, rotation, null);
                var model = clone.GetComponent<EnemyStatus>();
            }
        }
    }
}
