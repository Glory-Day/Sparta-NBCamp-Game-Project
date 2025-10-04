using System.Collections;
using System.Collections.Generic;
using Backend.Object.Character.Player;
using Backend.Object.Management;
using UnityEngine;

namespace Backend.Object.Projectile
{
    public class S0206Projectile : BaseProjectile
    {
        [Header("소멸 설정")]
        [SerializeField] private float _destroyTime = 1f; // 사라지기 전 대기 시간

        private void OnEnable()
        {
            _isHit = false;
            CheckForPlayer();
            StartCoroutine(DestroyAfterDelay());
        }



        private IEnumerator DestroyAfterDelay()
        {
            yield return new WaitForSeconds(_destroyTime);
            ObjectPoolManager.Release(gameObject);
        }

        // 디버깅 및 범위 시각화를 위해 OnDrawGizmos를 사용합니다.

    }
}
