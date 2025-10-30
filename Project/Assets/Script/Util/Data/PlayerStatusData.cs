using Backend.Util.Json;
using UnityEngine;

namespace Backend.Util.Data
{
    [CreateAssetMenu(menuName = "Scriptable Object/Data/Player Status Data")]
    public class PlayerStatusData : StatusData, ISerializable
    {
        [field: SerializeField] public float StaminaPoint { get; set; }

        [field: SerializeField] public int Level { get; set; }

        [field: SerializeField] public float LifePoint { get; set; }
        [field: SerializeField] public float ConcentrationPoint { get; set; }
        [field: SerializeField] public float EndurancePoint { get; set; }
        [field: SerializeField] public float PhysicalPoint { get; set; }
        [field: SerializeField] public float StrengthPoint { get; set; }
        [field: SerializeField] public float IntelligencePoint { get; set; }
        [field: SerializeField] public float LuckPoint { get; set; }

        [field: SerializeField] public float Soul { get;  set; }

        private const string FileName = "/Data/status.json";

        public void Load()
        {
            var data = JsonSerializer.Deserialize<PlayerStatusData>(Application.persistentDataPath + FileName);

            HealthPoint += data.HealthPoint;
            StaminaPoint += data.StaminaPoint;
            Speed += data.Speed;

            PhysicalDamage += data.PhysicalDamage;
            MagicalDamage += data.MagicalDamage;

            PhysicalDefense += data.PhysicalDefense;
            MagicalDefense += data.MagicalDefense;

            LifePoint += data.LifePoint;
            ConcentrationPoint += data.ConcentrationPoint;
            EndurancePoint += data.EndurancePoint;
            PhysicalPoint += data.PhysicalPoint;
            StrengthPoint += data.StrengthPoint;
            IntelligencePoint += data.IntelligencePoint;
            LuckPoint += data.LuckPoint;
        }

        public void Save()
        {
            JsonSerializer.Serialize(Application.persistentDataPath + FileName, this);
        }
    }
}
