using System.Collections.Generic;
using Backend.Util.Debug;
using UnityEngine;

namespace Backend.Object.Character
{
    public class Damage : MonoBehaviour
    {
        [HideInInspector] public float AttackDamage;
        private List<Collider> _hitColliders = new List<Collider>();
        public LayerMask hitLayer;

        //[SerializeField] Transform HitControlPosition;
        //public event System.Action<Collider> OnHitGiven;

        private void OnTriggerEnter(Collider other)
        {
            //적중 검사
            if (other.TryGetComponent(out IDamageable target))
            {
                if (!_hitColliders.Contains(other) && other.gameObject.layer == (int)Mathf.Log(hitLayer.value, 2))
                {
                    target.TakeDamage(AttackDamage);
                    _hitColliders.Add(other);
                }
            }
        }
        public void ResetState()
        {
            _hitColliders.Clear();
        }
        public void SetAttackDamage(float damage)
        {
            AttackDamage = damage;
        }
    }
}

