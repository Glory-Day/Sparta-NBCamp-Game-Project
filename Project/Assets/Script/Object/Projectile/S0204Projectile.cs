using System.Collections;
using System.Collections.Generic;
using Backend.Object.Management;
using UnityEngine;

namespace Backend.Object.Projectile
{
    public class S0204Projectile : BaseProjectile
    {
        [SerializeField] private float _disteyTime = 1f; // 사라지기 전 대기 시간
        private void OnEnable()
        {
            StartCoroutine(DestroyAfterDelay());

        }

        public override void Init(float damage, Vector3 position)
        {
            base.Init(damage, position);
            // _tagetPosition 위치를 바라보도록 설정
            Vector3 direction = (_tagetPosition - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }

        private IEnumerator DestroyAfterDelay()
        {
            yield return new WaitForSeconds(_disteyTime);
            ObjectPoolManager.Release(gameObject);
        }
    }
}
