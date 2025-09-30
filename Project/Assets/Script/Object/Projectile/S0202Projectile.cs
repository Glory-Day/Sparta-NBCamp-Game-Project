using System.Collections;
using Backend.Object.Management;
using UnityEngine;

namespace Backend.Object.Projectile
{
    public class S0202Projectile : BaseProjectile
    {
        [SerializeField] private float _disteyTime = 1f; // 사라지기 전 대기 시간

        // 스폰너인지 여부
        [SerializeField] private bool _isSpawner = false;

        // 데미지 수동 적용 여부
        [SerializeField]  private bool _isManualDamage = false;
        [SerializeField] private float _damage = 5f;
        private void OnEnable()
        {
            StartCoroutine(DestroyAfterDelay());

            if (_isManualDamage)
            {
                Init(_damage, Vector3.zero); // 예시로 데미지 10, 위치 (0,0,0)으로 초기화
            }

            if (_isSpawner)
            {
                transform.position = _tagetPosition;
            }
        }

        private IEnumerator DestroyAfterDelay()
        {
            yield return new WaitForSeconds(_disteyTime);
            ObjectPoolManager.Release(gameObject);
        }
    }
}

