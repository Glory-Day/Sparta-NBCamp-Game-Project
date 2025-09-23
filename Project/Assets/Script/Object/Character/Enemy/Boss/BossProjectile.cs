using System.Collections;
using System.Collections.Generic;
using Backend.Object.Management;
using Backend.Util.Debug;
using UnityEngine;

namespace Backend.Object.Character.Enemy.Boss
{
    public class BossProjectile : MonoBehaviour
    {
        private Transform _target;
        private float _speed = 7.0f;
        private float _damage = 0f;
        private float _remainingTime = 5f;
        private float _time = 0f;

        public void Initialize(Transform target, float damage)
        {
            _target = target;
            _damage = damage;
        }

        private void Update()
        {
            _time += Time.deltaTime;

            if (_target != null)
            {
                Vector3 direction = (_target.position - transform.position).normalized;

                transform.position += _speed * Time.deltaTime * direction;
                transform.rotation = Quaternion.LookRotation(direction); //일단 만들어둠
            }

            if(_remainingTime < _time)
            {
                ObjectPoolManager.Release(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IDamagable>(out var target))
            {
                target.TakeDamage(_damage);
                Debugger.LogSuccess($"{_damage}만큼 데미지를 가하였습니다.");
                ObjectPoolManager.Release(gameObject);
            }
            else //나중에 레이어로 체크할듯
            {
                ObjectPoolManager.Release(gameObject);
            }
            _time = 0f;
        }
    }
}
