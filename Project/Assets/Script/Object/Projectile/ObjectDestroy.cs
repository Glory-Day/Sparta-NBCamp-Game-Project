using Backend.Object.Management;
using UnityEngine;

namespace Backend.Object.Projectile
{
    public class ObjectDestroy : MonoBehaviour
    {
        public float ObjectDestroyTime = 2f;        // 이 오브젝트의 최대 수명 (초)

        private float time;                         // 오브젝트 생성(활성화) 시간을 기록

        private void OnEnable()
        {
            // 오브젝트가 활성화된 시간을 기록합니다.
            time = Time.time;
        }

        private void LateUpdate()
        {
            // 현재 시간이 (생성 시간 + 수명)을 초과하면
            if (Time.time > time + ObjectDestroyTime)
            {
                // 오브젝트 풀에 반환
                ObjectPoolManager.Release(gameObject);
            }
        }
    }
}

