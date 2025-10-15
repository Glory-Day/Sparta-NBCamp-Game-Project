using UnityEngine;

namespace Backend.Object.Projectile
{
    // 리기드바디 생성
    [RequireComponent(typeof(Rigidbody))]
    public class BaseProjectile : MonoBehaviour, IProjectile
    {
        private Rigidbody _rigidbody;

        protected Transform target;                             // 추적할 대상
        protected float damage;                                 // 데미지
        [SerializeField] protected float speed;                 // 이동 속도
        [SerializeField] protected float remainingTime;         // 지속 시간
        [SerializeField] protected float spawnDelay = 0f;       // 생성 후 대기 시간
        [SerializeField] protected float chasingTime;           // 추적 시간

        public LayerMask HitLayer;

        public void Initialized(Transform target, float damage, float speed, float spawnDelay, float chasingTime, float duration)
        {
            this.target = target;
            this.speed = speed;
            this.damage = damage;
            remainingTime = duration;
            this.chasingTime = chasingTime;
            this.spawnDelay = spawnDelay;

            OnInitialized();
        }
        public void Initialized(Transform target, float damage)
        {
            this.target = target;
            this.damage = damage;

            OnInitialized();
        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.useGravity = false; // 중력 비활성화
        }

        protected virtual void OnInitialized()
        {

        }
    }
}
