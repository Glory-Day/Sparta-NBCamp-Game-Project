using System.Collections;
using System.Collections.Generic;
using Backend.Object.Character;
using Backend.Object.Character.Player;
using Backend.Object.Management;
using UnityEngine;

namespace Backend.Object.Projectile
{
    public class S0201Projectile : BaseProjectile
    {
        [Header("이동 설정")]
        [SerializeField] private float durationTime = 0.5f;
        [SerializeField] private float moveDistance = 2.5f;

        [Header("판정 설정 (Box)")]
        [SerializeField] private Vector3 _boxSize = new Vector3(1f, 1f, 1f); // 데미지 판정 범위 (가로, 세로, 깊이)

        // 충돌 결과를 담을 배열 (미리 할당하여 가비지 생성 방지)
        private readonly Collider[] _hitColliders = new Collider[5]; // 여러 대상을 감지할 수 있도록 크기 조절
        // 이미 데미지를 입은 대상을 추적하기 위한 HashSet
        private readonly HashSet<Collider> _damagedTargets = new HashSet<Collider>();

        private void OnEnable()
        {
            _isHit = false;
            // 재사용을 위해 초기화
            _damagedTargets.Clear();
            StartCoroutine(MoveToPosition(transform.position + (transform.forward * moveDistance), durationTime));
        }

        /// <summary>
        /// 지정된 위치로 이동하며 충돌을 감지하는 코루틴
        /// </summary>
        private IEnumerator MoveToPosition(Vector3 targetPosition, float duration)
        {
            Vector3 startPosition = transform.position;
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                // 스무딩 효과를 적용한 이동
                float t = elapsedTime / duration;
                t = t * t * (3f - (2f * t));
                transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                elapsedTime += Time.deltaTime;

                // 이동하는 동안 매 프레임 충돌 검사
                CheckForPlayerInBox();

                yield return null;
            }
            transform.position = targetPosition;

            ObjectPoolManager.Release(gameObject);
        }

        /// <summary>
        /// 직육면체 범위 내의 플레이어를 감지하고 데미지를 줍니다.
        /// </summary>
        private void CheckForPlayerInBox()
        {
            // OverlapBoxNonAlloc을 사용하여 지정된 박스 영역 내의 _playerLayer 콜라이더를 감지합니다.
            int hitCount = Physics.OverlapBoxNonAlloc(transform.position, _boxSize / 2f, _hitColliders, transform.rotation, _playerLayer);

            // 감지된 콜라이더가 있고, 아직 공격이 성공하지 않았다면
            if (hitCount > 0 && !_isHit)
            {
                Debug.Log($"Player Hit! {gameObject.name}");
                // 첫 번째 감지된 콜라이더에서 PlayerStatus 컴포넌트를 가져옵니다.
                if (_hitColliders[0].TryGetComponent(out IDamagable playerDamagable))
                {
                    // 데미지를 주고, 공격 성공 플래그를 설정합니다.
                    playerDamagable.TakeDamage(_damage);
                    _isHit = true;

                    if (isDestroyed)
                    {
                        ObjectPoolManager.Release(gameObject);
                    }
                }
            }
        }
        protected override void OnDrawGizmos()
        {

        }

        // 디버깅 및 범위 시각화를 위해 OnDrawGizmos를 사용합니다.
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;

            // Gizmos가 로컬 좌표계에서 그려지도록 Matrix 설정
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.matrix = rotationMatrix;

            // 상자 형태의 와이어프레임 그리기
            Gizmos.DrawWireCube(Vector3.zero, _boxSize);
        }
    }
}
