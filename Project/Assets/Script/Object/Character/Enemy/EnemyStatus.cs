using Backend.Util.Data.StatusDatas;
using Backend.Util.Debug;
using UnityEngine;

namespace Backend.Object.Character.Enemy
{
    public class EnemyStatus : Status
    {
        [field: SerializeField] public StatusBossData BossStatus { get; private set; }
        private EnemyAnimationController _enemyAnimationController;

        private void Awake()
        {
            HealthPoint = BossStatus.HealthPoint;
            _enemyAnimationController = GetComponent<EnemyAnimationController>();
        }

        public override void TakeDamage(float damage)
        {
            base.TakeDamage(damage);

            if (_enemyAnimationController != null)
            {
                _enemyAnimationController.PlayHitCoroutine();
                Debugger.LogMessage("Hit Coroutine check");
            }
        }
    }
}
