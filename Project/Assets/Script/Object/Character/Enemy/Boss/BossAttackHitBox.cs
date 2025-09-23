using System.Collections;
using System.Collections.Generic;
using Backend.Object.Character;
using Backend.Util.Debug;
using UnityEngine;

public class BossAttackHitBox : MonoBehaviour
{
    [SerializeField] private BoxCollider _weaponCollider;
    [field: SerializeField] public Vector3 HitBoxSize { get; set; } = Vector3.one;
    private List<Collider> _hitTargets;
    public float Damage { get; set; }

    private void Awake()
    {
        if (_weaponCollider == null)
        {
            _weaponCollider = GetComponent<BoxCollider>();
            if (_weaponCollider == null)
            {
                Debugger.LogError($"Collider No");
            }
        }

        _weaponCollider.enabled = false;
        _weaponCollider.isTrigger = true;

        _hitTargets = new List<Collider>();
    }

    public void SetHitBoxSize(Vector3 size)
    {
        if(_weaponCollider != null)
        {
            _weaponCollider.size = new Vector3(size.x, size.y, size.z);
        }
    }

    public void EnableHitBox()
    {
        _hitTargets.Clear();
        _weaponCollider.enabled = true;
        Debugger.LogMessage("피격 판정 활성화");
    }

    public void DisableHitBox()
    {
        _weaponCollider.enabled = false;
        Debugger.LogMessage("피격 판정 비활성화");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_hitTargets.Contains(other))
        {
            return;
        }

        if (other.TryGetComponent<IDamagable>(out var target))
        {
            Debugger.LogSuccess($"{Damage}만큼 데미지를 가하였습니다.");
            target.TakeDamage(Damage);
        }
    }
}
