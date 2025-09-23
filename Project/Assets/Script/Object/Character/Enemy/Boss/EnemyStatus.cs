using System.Collections;
using System.Collections.Generic;
using Backend.Util.Data;
using Backend.Util.Data.StatusDatas;
using UnityEngine;

namespace Backend.Object.Character.Enemy.Boss
{
    public class EnemyStatus : Status
    {
        [field: SerializeField] public StatusBossData BossStatus { get; private set; }
    }
}
