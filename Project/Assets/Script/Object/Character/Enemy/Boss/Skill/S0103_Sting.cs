using System.Collections;
using Backend.Util.Data.ActionDatas;
using Backend.Util.Debug;
using UnityEngine;

namespace Backend.Object.Character.Enemy.Boss.Skill
{

    public class S0103_Sting : BossSkillBase
    {
        private EnemyMovementController _movement;
        [SerializeField] private float _rushSpeed = 20f;
        [SerializeField] private float _rushDuration = 0.8f;
        private void Start()
        {
            _movement = GetComponent<EnemyMovementController>();
        }

        protected override IEnumerator ExecuteSkillLogic(EnemyAnimationController animController, ActionBossData data) //animController 받아온다.
        {
            Debugger.LogSuccess("돌진 찌르기");

            Vector3 targetDir = _movement.GetDirection();
            Vector3 targetPos = _movement.Target.transform.position;

            targetDir.y = 0f;
            targetPos.y = 0f;

            Vector3 finalPos = targetPos - (targetDir * 1.5f);

            _movement.SetRotation();

            while (Vector3.Distance(transform.position, finalPos) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, finalPos, _rushSpeed * Time.deltaTime);
                yield return null;
            }
        }
    }
}
