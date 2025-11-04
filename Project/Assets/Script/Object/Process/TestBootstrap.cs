using System.Collections;
using System.Collections.Generic;
using Backend.Object.Character.Enemy;
using Backend.Object.Character.Player;
using Backend.Object.Management;
using Backend.Object.UI;
using Backend.Util.Data;
using Script.Object.UI;
using UnityEngine;

namespace Backend.Object.Process
{
    public class TestBootstrap : Process
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private PlayerConditionInformationBinder binder;
        [SerializeField] private PlayerEquipmentInformationBinder binder2;
        [SerializeField] private PlayerLevelStatusInformationBinder binder3;
        [SerializeField] private PlayerInventoryInformationBinder binder4;
        [SerializeField] private PlayerStatusInformationBinder binder5;

        [SerializeField] private ItemData[] testItem = new ItemData[4];
        public override void Run()
        {
            var position = transform.position;
            var rotation = transform.rotation;

            var clone = ObjectPoolManager.SpawnPoolObject(prefab, position, rotation, null);
            var model = clone.GetComponent<PlayerStatus>();

            Inventory[] _inventory = new Inventory[4];
            _inventory[0] = new Inventory(25, 25);
            _inventory[1] = new Inventory(6, 6);
            _inventory[2] = new Inventory(4, 4);
            _inventory[3] = new Inventory(4, 4);

            _inventory[0].items[0] = ScriptableObject.Instantiate(testItem[0]);
            _inventory[0].items[1] = ScriptableObject.Instantiate(testItem[1]);
            _inventory[0].items[2] = ScriptableObject.Instantiate(testItem[2]);
            _inventory[0].items[3] = ScriptableObject.Instantiate(testItem[3]);
            _inventory[0].items[4] = ScriptableObject.Instantiate(testItem[3]);

            Target = clone;

            binder.Bind(model);
            binder2.Bind(_inventory, model);
            binder3.Bind(model);
            binder4.Bind(_inventory[0], model);
            binder5.Bind(_inventory, model);
        }

        public GameObject Target { get; set; }
    }
}
