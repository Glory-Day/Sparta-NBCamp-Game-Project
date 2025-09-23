using System.Collections;
using System.Collections.Generic;
using Backend.Object.Character;
using Backend.Object.Character.Enemy.Boss;
using Backend.Object.Character.Enemy.Boss.Skill;
using Backend.Util.Data.ActionDatas;
using Backend.Util.Debug;
using UnityEngine;

namespace Backend.Object.Character.Enemy.Boss.Skill
{
    public class S0105_BloodSucking : BossSkillBase
    {
        protected override IEnumerator ExecuteSkillLogic(EnemyAnimationController animController, ActionBossData data)
        {
            Debugger.LogSuccess("흡혈 발동");
            yield return null;
        }
    }
}
