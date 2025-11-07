using System.Collections;
using System.Collections.Generic;
using Backend.Object.Character;
using Backend.Util.Debug;
using UnityEngine;

namespace Backend.Object.Projectile
{

    // 파티클 시간이 지나면 사라지는 스크립트 생성
    //[RequireComponent(typeof())]
    public class RangeProjectile : BaseProjectile
    {
        [Header("범위 설정")]
        [SerializeField] private float _radius = 1f;

        [Header("성장 효과")]
        [SerializeField] private bool useGrowingRadius = false; // 성장 효과 사용 여부
        [SerializeField] private float growthDuration = 1f;     // 최대 크기까지 성장하는 데 걸리는 시간
        [SerializeField] private float startRadius = 0.1f;      // 시작 크기

        private readonly Collider[] _hitColliders = new Collider[10];
        private readonly HashSet<int> _hits = new();
        private float _currentRadius;

        private EffectSoundPlayer _effectSoundPlayer;

        private void Awake()
        {
            _effectSoundPlayer = GetComponent<EffectSoundPlayer>();
        }

        private void OnEnable()
        {
            _hits.Clear();
            // 성장 효과 사용 여부에 따라 초기 반지름 설정
            _currentRadius = useGrowingRadius ? startRadius : _radius;

            if(_effectSoundPlayer != null)
            {
                _effectSoundPlayer.Play(0);
            }
        }

        protected override void OnInitialized()
        {
            if (useGrowingRadius)
            {
                StartCoroutine(GrowAndCheckRoutine());
            }
            else
            {
                CheckForTargets(_radius);
            }
        }

        private IEnumerator GrowAndCheckRoutine()
        {
            float elapsedTime = 0f;

            while (elapsedTime < growthDuration)
            {
                elapsedTime += Time.deltaTime;

                // growthDuration 동안 startRadius에서 _radius까지 선형적으로 증가
                float growthRatio = Mathf.Clamp01(elapsedTime / growthDuration);
                _currentRadius = Mathf.Lerp(startRadius, _radius, growthRatio);

                CheckForTargets(_currentRadius);

                yield return null; // 다음 프레임까지 대기
            }
            _currentRadius = _radius; // 최종 크기 보정
        }

        // 지정된 범위 내의 대상을 감지하고 데미지를 준다
        private void CheckForTargets(float currentRadius)
        {
            int hitCount = Physics.OverlapSphereNonAlloc(transform.position, currentRadius, _hitColliders, HitLayer);

            if (hitCount > 0)
            {
                for (var i = 0; i < hitCount; i++)
                {
                    var id = _hitColliders[i].gameObject.GetInstanceID();

                    // 아직 공격하지 않은 대상이고, IDamagable 컴포넌트가 있다면 공격
                    if (!_hits.Contains(id) && _hitColliders[i].TryGetComponent<IDamageable>(out var target))
                    {
                        target.TakeDamage(damage);
                        Debug.Log($"Player Hit! {gameObject.name}");
                        _hits.Add(id); // 공격한 대상으로 기록
                    }
                }
            }
        }

        private void OnDisable()
        {
            _hits.Clear();
        }

        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            // 에디터에서 현재 크기를 실시간으로 확인
            Gizmos.DrawWireSphere(transform.position, Application.isPlaying ? _currentRadius : _radius);
        }
    }
}
