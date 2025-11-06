using System.Collections.Generic;
using System.Linq;
using Backend.Util.Data;
using Script.Object.Character.Player;
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

        public void Bind(PlayerCharacterComposer composer)
        {
            var length = _spawners.Count;
            for (var i = 0; i < length; i++)
            {
                var trigger = _spawners[i].GetComponent<PlayerAnimationTrigger>();
                trigger.Composer = composer;
            }
        }

        public SpawnData GetSpawnData(int id)
        {
            return (from spawner in _spawners where spawner.Identifier == id select spawner.Data).FirstOrDefault();
        }
    }
}
