using System.Collections;
using System.Collections.Generic;
using Backend.Object.Management;
using UnityEngine;

namespace Backend.Object.Projectile
{
    public class ProjectileEffectReturn : MonoBehaviour
    {
        private ParticleSystem _particleSystem;

        private void OnEnable()
        {
            if (_particleSystem == null)
            {
                _particleSystem = GetComponentInChildren<ParticleSystem>();
            }

            float returnTime = Mathf.Max(_particleSystem.main.duration, _particleSystem.main.startLifetime.constantMax);

            CancelInvoke(nameof(ReturnToPool));
            Invoke(nameof(ReturnToPool), returnTime);
        }

        private void ReturnToPool()
        {
            ObjectPoolManager.Release(gameObject);
        }
    }
}
