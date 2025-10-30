using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Backend.Util.Data
{
    [System.Serializable]
    public struct GameObjectData
    {
        [field: Header("Transform Settings")]
        [field: SerializeField] public Vector3 Position { get; set; }
        [field: SerializeField] public Quaternion Rotation { get; set; }

        [field: Header("Type Settings")]
        [field: SerializeField] public SpawnData.GameObjectType Type { get; set; }
    }

    [CreateAssetMenu(fileName = "Spawn_Data", menuName = "Scriptable Object/Data/Spawn Data")]
    public class SpawnData : ScriptableObject
    {
        [SerializeField] private List<GameObjectData> data;

        public List<GameObjectData> PlayerCharacterData =>
            data.Where(i => i.Type == GameObjectType.PlayerCharacter).ToList();

        public List<GameObjectData> WarriorEnemyCharacterData =>
            data.Where(i => i.Type == GameObjectType.WarriorEnemyCharacter).ToList();

        public List<GameObjectData> ArcherEnemyCharacterData =>
            data.Where(i => i.Type == GameObjectType.ArcherEnemyCharacter).ToList();

        public List<GameObjectData> MiniBossEnemyCharacterData =>
            data.Where(i => i.Type == GameObjectType.MiniBossEnemyCharacter).ToList();

        public List<GameObjectData> BossEnemyCharacterData =>
            data.Where(i => i.Type == GameObjectType.BossEnemyCharacter).ToList();

        #region NESTED ENUMERABLE API

        public enum GameObjectType
        {
            PlayerCharacter,
            WarriorEnemyCharacter,
            ArcherEnemyCharacter,
            MiniBossEnemyCharacter,
            BossEnemyCharacter,
        }

        #endregion
    }
}
