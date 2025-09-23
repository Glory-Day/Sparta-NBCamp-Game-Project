using System.Collections;
using Backend.Util.Data.ActionDatas;
using Backend.Util.Debug;
using UnityEngine;

namespace Backend.Object.Character.Enemy.Boss.Skill
{
    public class S0104_EnergyExplose : BossSkillBase
    {
        [field: SerializeField] public float AttackRadius { get; set; }
        protected override IEnumerator ExecuteSkillLogic(EnemyAnimationController animController, ActionBossData data)
        {
            Debugger.LogSuccess("에너지 폭발 발동");
            Collider[] hitTargets = Physics.OverlapSphere(transform.position, AttackRadius);

            foreach (var hit in hitTargets)
            {
                if (hit.TryGetComponent<IDamagable>(out var target))
                {
                    if (hit.gameObject == gameObject)
                    {
                        continue;
                    }
                    target.TakeDamage(-SkillData.Damage);
                    Debugger.LogMessage($"{SkillData.Damage}만큼 데미지를 가하였습니다.");
                }
            }
            yield return null;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, AttackRadius);
        }
    }
}
