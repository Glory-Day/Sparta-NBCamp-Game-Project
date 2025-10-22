using System;
using Backend.Object.Character.Player;
using Backend.Object.Management;
using Script.Object.UI;
using UnityEngine;

namespace Script.Test
{
    [Serializable]
    public class PlayerCharacterSpawner : Progress
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private PlayerConditionInformationBinder binder;

        public override void Boot()
        {
            var position = transform.position;
            var rotation = transform.rotation;

            var clone = ObjectPoolManager.SpawnPoolObject(prefab, position, rotation, null);
            var model = clone.GetComponent<PlayerStatus>();

            Target = clone;

            binder.Bind(model);
        }

        public GameObject Target { get; set; }
    }
}
