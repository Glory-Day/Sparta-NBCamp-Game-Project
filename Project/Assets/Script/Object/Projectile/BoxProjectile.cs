using System.Collections.Generic;
using Backend.Object.Character;
using UnityEngine;
namespace Backend.Object.Projectile
{
    public class BoxProjectile : BaseProjectile
    {
        // 박스 범위
        [SerializeField] private Vector3 _boxSize = new Vector3(1f, 1f, 1f);
        private readonly Collider[] _hitColliders = new Collider[10];

        private HashSet<int> _hits = new();
        private bool _isHit;

        private void OnEnable()
        {
            _isHit = false;
        }

        protected override void OnInitialized()
        {
            CheckForPlayer();
        }

        // 지정된 범위 내의 플레이어를 감지하고 데미지를 준다
        private void CheckForPlayer()
        {
            // OverlapSphereNonAlloc을 사용하여 지정된 위치(_radius)에 있는 HitLayer 콜라이더를 감지
            int hitCount = Physics.OverlapBoxNonAlloc(transform.position, _boxSize, _hitColliders, transform.rotation, HitLayer);

            // 감지된 콜라이더가 있고, 아직 공격이 성공하지 않았다면
            if (hitCount > 0 && !_isHit)
            {
                for (var i = 0; i < hitCount; i++)
                {
                    var id = _hitColliders[i].gameObject.GetInstanceID();

                    if (!_hits.Contains(id) && _hitColliders[i].TryGetComponent<IDamageable>(out var target))
                    {
                        target.TakeDamage(damage);
                        Debug.Log($"Player Hit! {gameObject.name}");
                    }

                    _hits.Add(id);
                }

                _isHit = true;
            }
        }

        private void OnDisable()
        {
            _hits.Clear();
        }

        protected virtual void OnDrawGizmos()
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
