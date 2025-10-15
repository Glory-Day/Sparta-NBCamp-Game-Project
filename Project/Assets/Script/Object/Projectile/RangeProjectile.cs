using System;
using System.Collections;
using System.Collections.Generic;
using Backend.Object.Character;
using Backend.Object.Management;
using Backend.Util.Debug;
using Unity.VisualScripting;
using UnityEngine;

namespace Backend.Object.Projectile
{

    // 파티클 시간이 지나면 사라지는 스크립트 생성
    //[RequireComponent(typeof())]
    public class RangeProjectile : BaseProjectile
    {
        [SerializeField] private float _radius = 1f;
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
            int hitCount = Physics.OverlapSphereNonAlloc(transform.position, _radius, _hitColliders, HitLayer);

            // 감지된 콜라이더가 있고, 아직 공격이 성공하지 않았다면
            if (hitCount > 0 && !_isHit)
            {
                for (var i = 0; i < hitCount; i++)
                {
                    var id = _hitColliders[i].gameObject.GetInstanceID();

                    if (!_hits.Contains(id) && _hitColliders[i].TryGetComponent<IDamagable>(out var target))
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
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}
