using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Backend.Object.Character;
using Backend.Object.Management;
using Backend.Util.Debug;
using UnityEngine;

namespace Backend.Object.Projectile
{
    public class MoveProjectile : BaseProjectile
    {
        [Header("추적 설정")]
        [SerializeField] private bool shouldTrackTarget = true;         // 타겟을 추적할지 여부

        [Header("타겟 바라보기 설정")]
        [SerializeField] private bool lookAtTargetOnInit = true;        // 초기화 시 타겟을 바라보게 할지 여부

        [Header("RangeProjectile 생성")]
        [SerializeField] private bool isCreateRangeProjectile = false;  // RangeProjectile을 생성할지 여부
        [SerializeField] private GameObject rangeProjectilePrefab;      // 생성할 RangeProjectile 프리팹

        private float _time = 0f;                                       // 경과 시간
        private void OnEnable()
        {
            _time = 0f;
        }

        private void Update()
        {
            _time += Time.deltaTime;

            if (spawnDelay >= _time)
            {
                return;
            }

            if (target != null && chasingTime >= _time && shouldTrackTarget)
            {
                Vector3 direction = (target.position - transform.position).normalized;

                transform.position += speed * Time.deltaTime * direction;
                transform.rotation = Quaternion.LookRotation(direction);
            }
            else
            {
                transform.position += speed * Time.deltaTime * transform.forward;
            }

            if (remainingTime < _time)
            {
                ObjectPoolManager.Release(gameObject);
            }
        }

        protected override void OnInitialized()
        {
            LookAtTarget();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IDamagable>(out var target) && other.gameObject.layer == (int)Mathf.Log(HitLayer.value, 2))
            {
                target.TakeDamage(damage);
                Debugger.LogProgress($"{damage}만큼 데미지를 가하였습니다.");
            }
            Debug.Log($"Hit Object : {other.name}");
            CreateRangeProjectile();
            ObjectPoolManager.Release(gameObject);
        }

        // RangeProjectile 생성
        private void CreateRangeProjectile()
        {
            if (isCreateRangeProjectile)
            {
                var go = ObjectPoolManager.SpawnPoolObject(rangeProjectilePrefab, transform.position, transform.rotation, this.transform.parent);
                go.GetComponent<RangeProjectile>()?.Initialized(target, damage);
            }
        }

        // 타겟을 바라보게 하는 메서드
        private void LookAtTarget()
        {
            if (target != null && lookAtTargetOnInit)
            {
                Vector3 direction = (target.position - transform.position).normalized;
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }
}
