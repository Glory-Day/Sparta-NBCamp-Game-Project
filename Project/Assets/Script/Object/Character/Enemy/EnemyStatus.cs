using System;
using System.Collections;
using Backend.Object.Character.Player;
using Backend.Object.Management;
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

        [Header("Sound")]
        [SerializeField] private int[] takeDamageSfxIndex;
        [SerializeField] private int deathSfxIndex = -1;

        // 일반 몬스터 인지
        public bool IsNormalMonster = false;

        public bool IsParryable = false;

        // 히트 후 움직이기까지 시간
        public float HitStunTime = 0.5f;

        private bool _isHitAnimationPlaying = false;

        protected override void Awake()
        {
            base.Awake();

            _enemyAnimationController = GetComponent<EnemyAnimationController>();

        }

        public override void TakeDamage(float damage, Vector3? position = null)
        {
            base.TakeDamage(damage, null);

            if (currentHealthPoint <= 0)
            {
                currentHealthPoint = 0;
                OnDeath?.Invoke();

                if (_effectSoundPlayer != null && deathSfxIndex > 0)
                {
                    _effectSoundPlayer.Play(deathSfxIndex);
                }

                // 플레이어에게 소울 지급
                GetComponent<EnemyMovementController>().Target.GetComponent<PlayerStatus>().TakeSoul(BossStatus.SoulPoint);

                StartCoroutine(FadeOutAndDestroy(5f));
                return;
            }

            if (_effectSoundPlayer != null && takeDamageSfxIndex.Length != 0)
            {
                _effectSoundPlayer.Play(takeDamageSfxIndex[UnityEngine.Random.Range(0, takeDamageSfxIndex.Length)]);
            }

            if (IsNormalMonster && !_isHitAnimationPlaying)
            {
                StartCoroutine(HitCoroutine());
                OnHit?.Invoke();
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

        //죽었을때 서서히 사라지게 하는 함수
        private IEnumerator FadeOutAndDestroy(float delay)
        {
            yield return new WaitForSeconds(delay);
            // 오브젝트 풀에 반환
            ObjectPoolManager.Release(gameObject);
        }

        // 히트 애니메이션 중복 방지용 코루틴
        private IEnumerator HitCoroutine()
        {
            _isHitAnimationPlaying = true;
            yield return new WaitForSeconds(HitStunTime);
            _isHitAnimationPlaying = false;
        }
    }
}
