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
    public class TestBootstrap : TestProcess
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private PlayerConditionInformationBinder binder;
        [SerializeField] private PlayerEquipmentInformationBinder binder2;
        [SerializeField] private PlayerLevelStatusInformationBinder binder3;
        [SerializeField] private PlayerInventoryInformationBinder binder4;
        [SerializeField] private PlayerStatusInformationBinder binder5;
        [SerializeField] private PlayerBattleSlotInformationBinder binder6;

        [SerializeField] private ItemData[] testItem = new ItemData[4];
        public override void Run()
        {
            var position = transform.position;
            var rotation = transform.rotation;

            var clone = ObjectPoolManager.SpawnPoolObject(prefab, position, rotation, null);
            var model = clone.GetComponent<PlayerStatus>();

            //_inventory[0] = 플레이어 인벤토리
            //_inventory[1] = 무기 아이템 인벤토리
            //_inventory[2] = 방어구 아이템 인벤토리
            //_inventory[3] = 소모품 아이템 인벤토리

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
            binder6.Binder(_inventory);
        }

        public GameObject Target { get; set; }
    }
}
