﻿using System.Collections;
using System.Collections.Generic;
using Backend.Object.Management;
using UnityEngine;

namespace Backend.Object.Projectile
{
    public class S0205Projectile : BaseProjectile
    {
        [SerializeField] private float _disteyTime = 1f; // 사라지기 전 대기 시간
        private void OnEnable()
        {
            _isHit = false;
            CheckForPlayer();
            StartCoroutine(DestroyAfterDelay());
        }

        private IEnumerator DestroyAfterDelay()
        {
            yield return new WaitForSeconds(_disteyTime);
            ObjectPoolManager.Release(gameObject);
        }
    }
}
