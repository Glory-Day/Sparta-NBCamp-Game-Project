using System;
using Backend.Util.Data.StatusDatas;
using Backend.Util.Debug;
using Backend.Util.Presentation;
using UnityEngine;

namespace Backend.Object.Character.Enemy
{
    public enum EnemyState
    {
        Idle,
        Attack,
        Move,
        Parry,
    }
    public class EnemyStatus : Status
    {
        [field: SerializeField] public StatusBossData BossStatus { get; private set; }
        private EnemyAnimationController _enemyAnimationController;

        // 플레이어가 죽었을 때 이벤트
        public event Action OnEnemyDeath;
        public bool IsParryable = false;

        protected override void Awake()
        {
            base.Awake();

            _enemyAnimationController = GetComponent<EnemyAnimationController>();
        }

        public override void TakeDamage(float damage)
        {
            base.TakeDamage(damage);

            if (currentHealthPoint <= 0)
            {
                currentHealthPoint = 0;
                OnEnemyDeath?.Invoke();
                OnEnemyDeath = null;
            }

            if (_enemyAnimationController != null)
            {
                _enemyAnimationController.PlayHitCoroutine();
                Debugger.LogMessage("Hit Coroutine check");
            }
        }

        public void SetParry(bool parry)
        {
            IsParryable = parry;
        }
    }
}
