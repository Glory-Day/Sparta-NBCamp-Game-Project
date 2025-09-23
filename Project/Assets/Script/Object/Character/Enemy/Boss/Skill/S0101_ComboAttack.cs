using System.Collections;
using System.Collections.Generic;
using Backend.Util.Data.ActionDatas;
using Backend.Util.Debug;
using UnityEngine;

namespace Backend.Object.Character.Enemy.Boss.Skill
{
    public class S0101_ComboAttack : BossSkillBase
    {
        [SerializeField] private List<ActionBossData> _combos;
        [SerializeField] private float _delay = 0.2f;
        [field: SerializeField] public Vector3 AttackSize { get; set; } = Vector3.one;

        protected override IEnumerator ExecuteSkillLogic(EnemyAnimationController animController, ActionBossData data)
        {
            Debugger.LogSuccess("콤보 공격 발동");
            Vector3 originSize = _hitbox.HitBoxSize;

            _hitbox.SetHitBoxSize(AttackSize);
            _hitbox.Damage = SkillData.Damage;

            yield return new WaitForSeconds(animController.GetAnimationClipLength(0));

            for (int i = 0; i < _combos.Count; i++)
            {
                var skill = _combos[i];
                _hitbox.Damage = skill.Damage;
                animController.SetAnimationTrigger(skill.ID);

                yield return null;

                float animDuration = animController.GetAnimationClipLength(0);

                Debugger.LogMessage($"{animDuration}, {skill.ID}");

                yield return new WaitForSeconds(animDuration);

                float time = 0f;
                while (time < animDuration - 0.2f)
                {
                    time += Time.deltaTime;
                    //_movement.SetLerpRotation();
                    yield return null;
                }

                if (_delay > 0)
                {
                    yield return new WaitForSeconds(_delay);
                }
            }
            _hitbox.SetHitBoxSize(originSize);
        }
    }
}
