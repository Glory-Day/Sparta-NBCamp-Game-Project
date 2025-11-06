using Backend.Util.Data;
using UnityEngine;

namespace Backend.Object.NPC
{
    public class PlayerCharacterSpawner : MonoBehaviour
    {
        [field: Header("Data Settings")]
        [field: SerializeField] public SpawnData Data { get; private set; }

        [field: Header("Identifier Settings")]
        [field: SerializeField] public string Identifier { get; private set; }
    }
}
