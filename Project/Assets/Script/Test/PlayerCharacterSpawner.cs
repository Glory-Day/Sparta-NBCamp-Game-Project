using System;
using System.Collections.Generic;
using Backend.Object.Character.Player;
using Backend.Object.Management;
using Backend.Object.UI;
using Backend.Util.Data;
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
        [SerializeField] private PlayerInventoryInformationBinder binder3;

        [SerializeField] private ItemData[] testItemData = new ItemData[1];

        public override void Boot()
        {
            var position = transform.position;
            var rotation = transform.rotation;

            var clone = ObjectPoolManager.SpawnPoolObject(prefab, position, rotation, null);
            var model = clone.GetComponent<PlayerStatus>();
            var model2 = new Inventory(32, 81);

            for(int i = 0; i < testItemData.Length; i++)
            {
                model2.items[i] = testItemData[i];
            }

            Target = clone;

            binder.Bind(model);
            binder2.Bind(model);
            binder3.Bind(model2, model);
        }

        public GameObject Target { get; set; }
    }
}
