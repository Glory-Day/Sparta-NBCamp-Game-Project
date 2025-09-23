using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Backend.Util.Data.ActionDatas
{
    public enum RangeCheck
    {
        CloseRange,
        MidRange,
        LongRanged,
        None = 999
    }

    [CreateAssetMenu(fileName = "ActionBossData", menuName = "ActionData/ActionBossData")]
    public class ActionBossData : ActionData
    {
        [field: SerializeField] public RangeCheck ActionRange { get; private set; }
        [field: SerializeField] public bool IsParryable { get; private set; }
        [field: SerializeField] public float AttackWeight { get; private set; }
        [field: SerializeField] public float CoolDown { get; private set; }
    }
}

