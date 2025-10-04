using Backend.Object.Character;
using Backend.Object.Management;
using UnityEngine;

// 리기드바디 생성
[RequireComponent(typeof(Rigidbody))]
public class BaseProjectile : MonoBehaviour
{
    private Rigidbody _rigidbody;
    // 플레이어 위치
    protected Vector3 _tagetPosition;


    [Header("판정 설정")]
    [SerializeField] private float _radius = 1f; // 데미지 판정 범위
    [SerializeField] protected LayerMask _playerLayer; // 플레이어 레이어



    // 충돌 결과를 담을 배열 (미리 할당하여 가비지 생성 방지)
    private readonly Collider[] _hitColliders = new Collider[1];
    protected bool _isHit; // 중복 타격을 방지하기 위한 플래그
    // 파괴 여부
    [SerializeField] protected bool isDestroyed = false;

    protected float _damage;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = false; // 중력 비활성화
    }

    public virtual void Init(float damage, Vector3 position)
    {
        _damage = damage;
        _tagetPosition = position;
    }

    /// <summary>
    /// 지정된 범위 내의 플레이어를 감지하고 데미지를 줍니다.
    /// </summary>
    protected virtual void CheckForPlayer()
    {
        // OverlapSphereNonAlloc을 사용하여 지정된 위치(_radius)에 있는 _playerLayer 콜라이더를 감지합니다.
        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, _radius, _hitColliders, _playerLayer);

        // 감지된 콜라이더가 있고, 아직 공격이 성공하지 않았다면
        if (hitCount > 0 && !_isHit)
        {
            // 첫 번째 감지된 콜라이더에서 PlayerStatus 컴포넌트를 가져옵니다.
            if (_hitColliders[0].TryGetComponent(out IDamagable playerDamagable))
            {
                Debug.Log($"Player Hit! {gameObject.name}");
                // 데미지를 주고, 공격 성공 플래그를 설정합니다.
                playerDamagable.TakeDamage(_damage);
                _isHit = true;

                if (isDestroyed)
                {
                    ObjectPoolManager.Release(gameObject);
                }
            }
        }
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }

    public float SetDamage()
    {
        return _damage;
    }


}
