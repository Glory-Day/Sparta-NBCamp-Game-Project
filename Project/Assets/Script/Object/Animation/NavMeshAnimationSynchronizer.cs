using Backend.Object.Character.Enemy;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Backend.Object.Animation
{
    /// <summary>
    /// NavMeshAgent의 속도, 가속도에 맞춰 Animator의 "Speed" 파라미터를 자동으로 조절합니다.
    /// 코루틴을 사용하여 업데이트 주기를 제어하고 불필요한 연산을 줄여 최적화되었습니다.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(EnemyAnimationController))]
    public class NavMeshAnimationSynchronizer : MonoBehaviour
    {
        [Header("속도 설정")]
        [Tooltip("걷기 속도입니다. Animator의 Speed 파라미터 0.5에 해당합니다.")]
        [SerializeField] private float walkSpeed = 2.0f;
        [Tooltip("달리기 속도입니다. Animator의 Speed 파라미터 1.0에 해당합니다.")]
        [SerializeField] private float runSpeed = 5.0f;

        [Header("최적화 설정")]
        [Tooltip("애니메이션 동기화 주기입니다. 값이 클수록 성능 부담이 줄지만 반응이 늦어질 수 있습니다.")]
        [SerializeField] private float updateInterval = 0.1f;

        private NavMeshAgent _navMeshAgent;
        private EnemyAnimationController _animationController;
        private Coroutine _syncCoroutine;
        private WaitForSeconds _wait;

        private readonly int _speedHash = Animator.StringToHash("Speed");

        // SmoothDamp를 위한 참조 변수
        private float _currentVelocity;
        private float _currentAnimatorSpeed;

        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _animationController = GetComponent<EnemyAnimationController>();
            _wait = new WaitForSeconds(updateInterval);
        }

        private void OnEnable()
        {
            if (_syncCoroutine != null)
            {
                StopCoroutine(_syncCoroutine);
            }
            _syncCoroutine = StartCoroutine(SynchronizeAnimationRoutine());
        }

        private void OnDisable()
        {
            if (_syncCoroutine != null)
            {
                StopCoroutine(_syncCoroutine);
                _syncCoroutine = null;
            }
        }

        private IEnumerator SynchronizeAnimationRoutine()
        {
            yield return null;

            while (true)
            {
                float currentVelocityMagnitude = _navMeshAgent.velocity.magnitude;
                float targetAnimSpeed;

                // 1. 목표 애니메이션 속도 계산
                if (currentVelocityMagnitude <= walkSpeed)
                {
                    // [0, walkSpeed] 범위를 애니메이션 속도 [0, 0.5] 범위로 매핑합니다.
                    targetAnimSpeed = Mathf.InverseLerp(0, walkSpeed, currentVelocityMagnitude) * 0.5f;
                }
                else
                {
                    // [walkSpeed, runSpeed] 범위를 애니메이션 속도 [0.5, 1.0] 범위로 매핑합니다.
                    targetAnimSpeed = 0.5f + Mathf.InverseLerp(walkSpeed, runSpeed, currentVelocityMagnitude) * 0.5f;
                }

                // 2. 가속도 기반 전환 시간 계산
                // 가속도가 높을수록 전환 시간이 짧아져 애니메이션이 빠르게 반응합니다.
                // 가속도가 8(기본값)일 때 약 0.1초가 되도록 설정합니다.
                float smoothTime = (_navMeshAgent.acceleration > 0) ? 0.8f / _navMeshAgent.acceleration : 0.1f;

                // 3. 부드러운 전환 및 애니메이터 업데이트
                _currentAnimatorSpeed = Mathf.SmoothDamp(_currentAnimatorSpeed, targetAnimSpeed, ref _currentVelocity, smoothTime);

                // 계산된 값이 매우 작으면 0으로 처리하여 애니메이션을 완전히 멈춥니다.
                if (_currentAnimatorSpeed < 0.01f)
                {
                    _currentAnimatorSpeed = 0f;
                }

                _animationController.SetAnimationFloat(_speedHash, _currentAnimatorSpeed);

                yield return _wait;
            }
        }
    }
}
