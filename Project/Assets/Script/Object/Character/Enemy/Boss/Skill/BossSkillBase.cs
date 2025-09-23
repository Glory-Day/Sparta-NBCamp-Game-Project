using System;
using System.Collections;
using Backend.Util.Data.ActionDatas;
using UnityEngine;

namespace Backend.Object.Character.Enemy.Boss.Skill
{
    public abstract class BossSkillBase : MonoBehaviour
    {
        [field: SerializeField] public ActionBossData SkillData { get; protected set; }
        [SerializeField] protected GameObject projectilePrefab;
        [SerializeField] protected Transform[] projectileTransform;
    }
}
