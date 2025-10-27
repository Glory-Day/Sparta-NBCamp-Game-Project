using Backend.Object.Character.Enemy;
using Backend.Object.Management;
using Script.Object.UI;
using UnityEngine;

namespace Script.Test
{
    public class BossEnemyCharacterSpawner : Progress
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private BossEnemyConditionInformationBinder binder;
        [SerializeField] private PlayerCharacterSpawner playerSpawner;
        public override void Boot()
        {
            var position = transform.position;
            var rotation = transform.rotation;

            var clone = ObjectPoolManager.SpawnPoolObject(prefab, position, rotation, null);
            var component = clone.GetComponent<EnemyMovementController>();

            if(playerSpawner.Target == null)
            {
                Debug.LogError("Player Target is Null");
                return;
            }
            component.Target = playerSpawner.Target;

            var model = clone.GetComponent<EnemyStatus>();

            binder.Bind(model);
        }
    }
}
