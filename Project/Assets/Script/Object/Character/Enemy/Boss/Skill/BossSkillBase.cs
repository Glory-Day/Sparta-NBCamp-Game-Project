using System;
using System.Collections;
using Backend.Util.Data.ActionDatas;
using Backend.Util.Debug;
using UnityEngine;

namespace Backend.Object.Character.Enemy.Boss.Skill
{
    public abstract class BossSkillBase : MonoBehaviour
    {
        [field: SerializeField] public ActionBossData SkillData { get; protected set; }
        [SerializeField] protected BossAttackHitBox _hitbox;

        public event Action OnSkillEnd;
        public IEnumerator ExecuteSkill(EnemyAnimationController animController, ActionBossData data)
        {
            animController.SetAnimationTrigger(SkillData.ID);
            yield return new WaitForSeconds(0.01f);

            yield return StartCoroutine(ExecuteSkillLogic(animController, data));

            TriggerOnSkillEnd();
            yield return null;
        }

        // 실제 스킬 로직 구현
        protected abstract IEnumerator ExecuteSkillLogic(EnemyAnimationController animController, ActionBossData data);

        // 스킬 종료 이벤트 트리거
        protected void TriggerOnSkillEnd()
        {
            Debugger.LogMessage($"{SkillData.name} is SkillEnd");
            OnSkillEnd?.Invoke();
            OnSkillEnd = null;
        }

        private void OnDestroy()
        {
            OnSkillEnd = null;
        }

        private void Awake()
        {
            if (_hitbox == null)
            {
                _hitbox = GetComponentInChildren<BossAttackHitBox>();
            }
        }
    }
}
