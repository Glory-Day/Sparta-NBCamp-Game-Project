using System;
using System.Collections;
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

        // 플레이어가 죽었을 때 이벤트
        public event Action OnEnemyDeath;
        public bool IsParryable = false;

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
                OnEnemyDeath?.Invoke();
                OnEnemyDeath = null;
                StartCoroutine(FadeOutAndDestroy(5f));
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

            //// 모든 렌더러를 가져옵니다.
            //var renderers = GetComponentsInChildren<Renderer>();
            //float fadeDuration = 2f; // 2초 동안 사라짐
            //float elapsedTime = 0f;

            //// Dissolve 효과나 투명도 조절을 위한 초기 설정 (Shader에 따라 다름)
            //// 예: "_Fade" 라는 프로퍼티를 가진 셰이더를 사용한다고 가정
            //// foreach (var rend in renderers)
            //// {
            ////     rend.material.SetFloat("_Fade", 1f);
            //// }

            //while (elapsedTime < fadeDuration)
            //{
            //    elapsedTime += Time.deltaTime;
            //    float fadeValue = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);

            //    foreach (var rend in renderers)
            //    {
            //        // 사용하는 셰이더의 프로퍼티에 맞게 수정해야 합니다.
            //        // 예: rend.material.SetFloat("_Fade", fadeValue);

            //        // 간단한 투명도 조절 예시 (Transparent 셰이더 필요)
            //        Color color = rend.material.color;
            //        color.a = fadeValue;
            //        rend.material.color = color;
            //    }

            //    yield return null;
            //}
            // 오브젝트 풀에 반환
            ObjectPoolManager.Release(gameObject);
        }
    }
}
