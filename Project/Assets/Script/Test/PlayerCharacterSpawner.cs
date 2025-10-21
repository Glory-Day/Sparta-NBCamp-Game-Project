using Backend.Object.Character.Player;
using Backend.Object.Management;
using Script.Object.UI;
using UnityEngine;

namespace Script.Test
{
    public class PlayerCharacterSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private PlayerConditionInformationBinder binder;

        private void Awake()
        {
            var position = transform.position;
            var rotation = transform.rotation;

            var clone = ObjectPoolManager.SpawnPoolObject(prefab, position, rotation, null);
            var model = clone.GetComponent<PlayerStatus>();

            binder.Bind(model);
        }
    }
}
