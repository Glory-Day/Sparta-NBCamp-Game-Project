using System.Collections;
using Backend.Object.Character.Enemy.Boss;
using Backend.Object.Character.Enemy.Boss.Skill;
using Backend.Util.Data.ActionDatas;
using Backend.Util.Debug;
using UnityEngine;

public class S0102_Slash : BossSkillBase
{
    [field: SerializeField] public Vector3 AttackSize { get; set; } = Vector3.one;
    private EnemyMovementController _movement;

    private void Start()
    {
        _movement = GetComponent<EnemyMovementController>();
    }

    protected override IEnumerator ExecuteSkillLogic(EnemyAnimationController animController, ActionBossData data)
    {
        //Debugger.LogSuccess("휘두르기");

        //Vector3 originSize = _hitbox.HitBoxSize;
        //_hitbox.SetHitBoxSize(AttackSize);
        //_hitbox.Damage = SkillData.Damage;

        //float animDuration = animController.GetAnimationClipLength(0);
        //float time = 0f;
        //while(time < animDuration)
        //{
        //    _movement.SetLerpRotation();
        //}

        //_hitbox.SetHitBoxSize(originSize);
        yield return null;
    }
}
