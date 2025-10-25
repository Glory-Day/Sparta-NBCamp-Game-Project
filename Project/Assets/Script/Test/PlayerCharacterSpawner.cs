using System;
using Backend.Object.Character.Player;
using Backend.Object.Management;
using Backend.Object.UI;
using Script.Object.UI;
using UnityEngine;

namespace Script.Test
{
    [Serializable]
    public class PlayerCharacterSpawner : Progress
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private PlayerConditionInformationBinder binder;
        [SerializeField] private PlayerLevelStatusInformationBinder binder2;

        public override void Boot()
        {
            var position = transform.position;
            var rotation = transform.rotation;

            var clone = ObjectPoolManager.SpawnPoolObject(prefab, position, rotation, null);
            var model = clone.GetComponent<PlayerStatus>();

            Target = clone;

            binder.Bind(model);
            binder2.Bind(model);
        }

        public GameObject Target { get; set; }
    }
}
