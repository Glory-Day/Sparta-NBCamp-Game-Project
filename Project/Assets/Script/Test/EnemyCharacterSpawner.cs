using Backend.Object.Character.Enemy;
using Backend.Object.Management;
using Script.Object.UI;
using UnityEngine;

namespace Script.Test
{
    public class EnemyCharacterSpawner : Progress
    {
        [SerializeField] private GameObject prefab;

        public override void Boot()
        {
            var position = transform.position;
            var rotation = transform.rotation;

            var clone = ObjectPoolManager.SpawnPoolObject(prefab, position, rotation, null);
        }
    }
}
