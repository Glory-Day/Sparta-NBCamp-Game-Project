using System.Collections;
using System.Collections.Generic;
using Backend.Util.Data.ActionDatas;
using UnityEngine;

namespace Backend.Object.Character.Enemy.Boss.Skill
{
    public class S0107_BackStep : BossSkillBase
    {
        [SerializeField] private float _backStepSpeed = 10f;
        [SerializeField] private float _backStepDistance = 5f;
        protected override IEnumerator ExecuteSkillLogic(EnemyAnimationController animController, ActionBossData data)
        {
            Vector3 startPos = transform.TransformPoint(transform.position);
            Vector3 targetPos = startPos - (transform.forward * _backStepDistance);
            while (Vector3.Distance(transform.position, targetPos) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, _backStepSpeed * Time.deltaTime);
                yield return null;
            }

            transform.position = targetPos;
            yield return null;
        }
    }
}
