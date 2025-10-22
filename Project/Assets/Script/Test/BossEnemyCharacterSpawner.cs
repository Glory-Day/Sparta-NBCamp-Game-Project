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

        public override void Boot()
        {
            var position = transform.position;
            var rotation = transform.rotation;

            var clone = ObjectPoolManager.SpawnPoolObject(prefab, position, rotation, null);
            var component = clone.GetComponent<EnemyMovementController>();
            component.Target = GetComponent<PlayerCharacterSpawner>().Target;

            var model = clone.GetComponent<EnemyStatus>();

            binder.Bind(model);
        }
    }
}
