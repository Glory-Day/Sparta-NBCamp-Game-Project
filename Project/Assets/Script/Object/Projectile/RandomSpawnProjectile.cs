using System.Collections;
using System.Collections.Generic;
using Backend.Object.Management;
using Backend.Object.Projectile;
using UnityEngine;

namespace Backend.Object.Projectile
{
    public class RandomSpawnProjectile : BaseProjectile
    {
        [Header("생성할 오브젝트들")]
        public GameObject[] makeObjs;

        [Header("생성 타이밍 설정")]
        public float startDelay;          // 오브젝트 생성을 시작하기 전의 초기 딜레이 (초)
        public int makeCount;             // 오브젝트를 생성할 총 횟수
        public float makeDelay;           // 각 생성 이벤트 사이의 딜레이 (초)

        [Header("랜덤 값 설정")]
        public Vector3 randomPos;         // 생성 위치에 적용될 랜덤 범위 (x, y, z 각각 ± 범위)
        public Vector3 randomRot;         // 생성 회전값에 적용될 랜덤 범위 (x, y, z 각각 ± 범위)
        public Vector3 randomScale;       // 생성 스케일에 적용될 랜덤 범위 (x, y, z 각각 ± 범위)

        [Header("타겟 바라보기 설정")]
        [SerializeField] private bool lookAtTargetOnInit = true;        // 초기화 시 타겟을 바라보게 할지 여부

        private float _Time;               // 스크립트 시작 시간을 기록 (초기 딜레이 계산용)
        private float _Time2;              // 마지막 생성 시간을 기록 (생성 간격 계산용)
        private float _count;              // 현재까지 생성한 횟수를 카운트
        private float _scalefactor;        // 씬의 전역 스케일 값 (위치 랜덤 범위에 적용)

        private void OnEnable()
        {
            // 시작 시간과 마지막 생성 시간을 현재 시간으로 초기화
            _Time = _Time2 = Time.time;
            // 씬의 전역 스케일 값을 가져옴
            _scalefactor = VariousEffectsScene.m_gaph_scenesizefactor;
        }

        private void Update()
        {
            // 초기 시작 딜레이가 지났는지 확인
            if (Time.time > _Time + startDelay)
            {
                // (마지막 생성 시간으로부터 생성 딜레이가 지났고) && (아직 생성 횟수가 목표 횟수보다 적은지) 확인
                if (Time.time > _Time2 + makeDelay && _count < makeCount)
                {
                    // 1. 랜덤 위치 계산
                    // 기본 위치 + (랜덤 벡터 * 스케일 팩터)
                    Vector3 m_pos = transform.position + GetRandomVector(randomPos) * _scalefactor;

                    // 2. 랜덤 회전 계산
                    // 기본 회전 * 랜덤 회전 오프셋
                    Quaternion m_rot = transform.rotation * Quaternion.Euler(GetRandomVector(randomRot));

                    // 3. m_makeObjs 배열에 있는 모든 프리팹에 대해 생성 실행
                    for (int i = 0; i < makeObjs.Length; i++)
                    {
                        // 오브젝트 풀에서 오브젝트 생성
                        GameObject obj = ObjectPoolManager.SpawnPoolObject(makeObjs[i], m_pos, m_rot, transform.parent);

                        obj.transform.parent = transform.parent; // 부모 오브젝트로 설정

                        obj.GetComponent<BaseProjectile>()?.Initialized(target, damage);

                        // 프리팹의 기본 스케일 + 랜덤 스케일 값
                        Vector3 m_scale = makeObjs[i].transform.localScale + GetRandomVector2(randomScale);

                        obj.transform.localScale = m_scale; // 계산된 스케일 적용
                    }

                    // 마지막 생성 시간을 현재 시간으로 갱신
                    _Time2 = Time.time;
                    // 생성 횟수 1 증가
                    _count++;
                }
            }

            if (_count >= makeCount)
            {
                ObjectPoolManager.Release(gameObject);
            }
        }

        protected override void OnInitialized()
        {
            LookAtTarget();
        }

        private float GetRandomValue(float value)
        {
            return Random.Range(-value, value);
        }

        private Vector3 GetRandomVector(Vector3 value)
        {
            Vector3 result;
            result.x = GetRandomValue(value.x);
            result.y = GetRandomValue(value.y);
            result.z = GetRandomValue(value.z);
            return result;
        }

        private float GetRandomValue2(float value)
        {
            return Random.Range(0, value);
        }

        private Vector3 GetRandomVector2(Vector3 value)
        {
            Vector3 result;
            result.x = GetRandomValue2(value.x);
            result.y = GetRandomValue2(value.y);
            result.z = GetRandomValue2(value.z);
            return result;
        }
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
