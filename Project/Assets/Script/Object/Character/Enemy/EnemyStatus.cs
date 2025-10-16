using System;
using Backend.Util.Data.StatusDatas;
using Backend.Util.Debug;
using UnityEngine;

namespace Backend.Object.Character.Enemy
{
    public class EnemyStatus : Status
    {
        [field: SerializeField] public StatusBossData BossStatus { get; private set; }
        private EnemyAnimationController _enemyAnimationController;

        // 플레이어가 죽었을 때 이벤트
        public event Action OnEnemyDeath;

        private void Awake()
        {
            HealthPoint = BossStatus.HealthPoint;
            _enemyAnimationController = GetComponent<EnemyAnimationController>();
        }

        public override void TakeDamage(float damage)
        {
            base.TakeDamage(damage);

            if (HealthPoint <= 0)
            {
                HealthPoint = 0;
                OnEnemyDeath?.Invoke();
                OnEnemyDeath = null;
            }

            if (_enemyAnimationController != null)
            {
                _enemyAnimationController.PlayHitCoroutine();
                Debugger.LogMessage("Hit Coroutine check");
            }
        }
    }
}
