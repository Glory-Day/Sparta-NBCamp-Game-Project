using Backend.Util.Json;
using UnityEngine;

namespace Backend.Util.Json.Data
{
    [System.Serializable]
    public class UserData
    {
        [SerializeField] public int SceneIndex { get; set; } = 1;
        [SerializeField] public int SpawnerIndex { get; set; }
    }
}
