using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Backend.Object.Character.Enemy
{
    public class FieldOfView : MonoBehaviour
    {
        // 시야 영역의 반지름과 시야 각도
        public float viewRadius;
        [Range(0, 360)]
        public float viewAngle;

        // 마스크 2종
        public LayerMask targetMask;

        // Target mask에 ray hit된 transform을 보관하는 리스트
        public List<Transform> visibleTargets = new List<Transform>();

        // 한번 타겟이 들어오면 시야 반경 밖으로 나가도 헤제 되지 않는 bool
        [SerializeField] private bool _keepTargetOutOfViewRadius = false;
        private EnemyMovementController _movementController;
        private EnemyStatus _enemyStatus;

        private void Awake()
        {
            _movementController = GetComponent<EnemyMovementController>();
            _enemyStatus = GetComponent<EnemyStatus>();

            viewRadius = _enemyStatus.BossStatus.AttackRange[(_enemyStatus.BossStatus.AttackRange.Length - 1)];
        }

        private void Start()
        {
            // 0.2초 간격으로 코루틴 호출
            StartCoroutine(FindTargetsWithDelay(0.2f));
        }

        private IEnumerator FindTargetsWithDelay(float delay)
        {
            WaitForSeconds wait = new WaitForSeconds(delay);
            while (true)
            {
                yield return wait;
                FindVisibleTargets();
            }
        }

        void FindVisibleTargets()
        {
            visibleTargets.Clear();

            // 현재 타겟이 있고, 시야 반경을 벗어났는지 확인
            if (_movementController.Target != null)
            {
                float dstToTarget = Vector3.Distance(transform.position, _movementController.Target.transform.position);
                if (dstToTarget > viewRadius && !_keepTargetOutOfViewRadius)
                {
                    // 타겟이 반경을 벗어나면 타겟 해제
                    _movementController.Target = null;
                }
                // 타겟이 여전히 유효하면, 새로운 타겟을 찾을 필요 없음
                else
                {
                    // 시각화를 위해 현재 타겟을 리스트에 추가
                    visibleTargets.Add(_movementController.Target.transform);
                    return;
                }
            }

            // 새로운 타겟 탐색
            // viewRadius를 반지름으로 한 원 영역 내 targetMask 레이어인 콜라이더를 모두 가져옴
            Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

            for (int i = 0; i < targetsInViewRadius.Length; i++)
            {
                Transform target = targetsInViewRadius[i].transform;
                Vector3 dirToTarget = (target.position - transform.position).normalized;

                // 플레이어와 forward와 target이 이루는 각이 설정한 각도 내라면
                if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
                {
                    float dstToTarget = Vector3.Distance(transform.position, target.position);
                    // 새로운 타겟을 발견하면 EnemyMovementController에 설정
                    _movementController.Target = target.gameObject;
                    visibleTargets.Add(target);

                    // 플레이어는 한 명이므로 첫 타겟 발견 시 반복 종료
                    break;
                }
            }
        }

        // y축 오일러 각을 3차원 방향 벡터로 변환한다.
        // 원본과 구현이 살짝 다름에 주의. 결과는 같다.
        public Vector3 DirFromAngle(float angleDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
            {
                angleDegrees += transform.eulerAngles.y;
            }

            return new Vector3(Mathf.Cos((-angleDegrees + 90) * Mathf.Deg2Rad), 0, Mathf.Sin((-angleDegrees + 90) * Mathf.Deg2Rad));
        }
    }
}
