using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Backend.Util.Data.StatusDatas
{
    [CreateAssetMenu(fileName = "StatusBossData", menuName = "StatusData/StatusBossData")]
    public class StatusBossData : StatusData
    {
        [field: SerializeField] public float ChasingSpeed { get; private set; }
        [field: SerializeField] public float TurnSpeed { get; private set; }
        [field: SerializeField] public float[] AttackRange { get; private set; }
        [field: SerializeField] public float SoulPoint { get; private set; }
    }
}
