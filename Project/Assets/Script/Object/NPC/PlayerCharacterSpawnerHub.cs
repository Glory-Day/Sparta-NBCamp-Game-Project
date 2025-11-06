using System.Collections.Generic;
using System.Linq;
using Backend.Util.Data;
using UnityEngine;

namespace Backend.Object.NPC
{
    public class PlayerCharacterSpawnerHub : MonoBehaviour
    {
        private List<PlayerCharacterSpawner> _spawners = new ();

        private void Awake()
        {
            var spawners = GetComponentsInChildren<PlayerCharacterSpawner>();

            var length = spawners.Length;
            for (var i = 0; i < length; i++)
            {
                _spawners.Add(spawners[i]);
            }
        }

        private void OnDestroy()
        {
            _spawners.Clear();
            _spawners = null;
        }

        public SpawnData GetSpawnData(string id)
        {
            return (from spawner in _spawners where spawner.Identifier == id select spawner.Data).FirstOrDefault();
        }
    }
}
