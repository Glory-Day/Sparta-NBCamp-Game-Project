using System.Collections;
using System.Collections.Generic;
using Backend.Object.Character;
using Backend.Util;
using Backend.Util.Data.ActionDatas;
using UnityEngine;

namespace Backend.Object.Character.Enemy
{
    public class EnemyCombatController : MonoBehaviour
    {
        [field: SerializeField] public ActionBossData[] ActionDatas { get; private set; }
        [field: SerializeField] public ActionBossData ActionData { get; set; }
        [SerializeField] private DamageSender _damageSender;
        [SerializeField] private DamageSender[] _damageSenders;
        [SerializeField] private EffectController _effect;
        [SerializeField] private EffectController[] _effects;
        public readonly Dictionary<string, CoolDownTimer> ActionCoolTimer = new(); // 스킬 쿨다운 타이머 딕셔너리

        private EnemyStatus _enemyStatus;

        public void Start()
        {
            _enemyStatus = GetComponent<EnemyStatus>();
            foreach (var action in ActionDatas)
            {
                ActionCoolTimer.Add(action.ID, new CoolDownTimer(action.CoolDown));
            }
        }

        public void StartAttack()
        {
            _damageSender.Damage = ActionData.Damage * _enemyStatus.BossStatus.PhysicalDamage;
            _damageSender.StartDetection();
        }

        public void StartAttackOf(int attackNum)
        {
            //_weapon.StartAttack(ActionDatas[attackNum].Damage);
        }

        public void EndAttack()
        {
            if (_damageSender != null)
            {
                _damageSender.StopDetection();
            }
        }

        public void SetWeapon(int weaponNum)
        {
            _damageSender = _damageSenders[weaponNum];
        }

        public void StartEffect()
        {
            _effect.SpawnEffect();
        }

        public void EndEffect()
        {
            if (_effect != null)
            {
                _effect.ReleaseEffect();
            }
        }

        public void SetEffect(int effectNum)
        {
            _effect = _effects[effectNum];
        }
    }

}
