using System;
using Backend.Util.Data;
using Backend.Util.Presentation;
using UnityEngine;

namespace Backend.Object.Character
{
    public class Status : MonoBehaviour, IDamageable, IModel
    {
        [Header("Data References")]
        [SerializeField] public StatusData data;

        [Header("Health Point Information")]
        [SerializeField] public float currentHealthPoint;
        [SerializeField] public float maximumHealthPoint;


        public Action OnDeath;
        public Action OnHit;
        protected EffectSoundPlayer _effectSoundPlayer;
        protected bool isDead = false;
        protected virtual void Awake()
        {

            _effectSoundPlayer = GetComponent<EffectSoundPlayer>();
        }

        protected virtual void OnEnable()
        {
            currentHealthPoint = data.HealthPoint;
            maximumHealthPoint = data.HealthPoint;
            HealthPointChanged?.Invoke(NormalizedHealthPoint);
            isDead = false;
        }

        public virtual void TakeDamage(float damage, Vector3? position = null)
        {
            //TODO: 현재 체력과 최대 체력을 구분해야 하기 때문에 해당 기능을 수정해야 한다.
            currentHealthPoint -= damage;

            HealthPointChanged?.Invoke(NormalizedHealthPoint);
        }

        public event Action<float> HealthPointChanged;

        private float NormalizedHealthPoint => currentHealthPoint / maximumHealthPoint;

        public bool IsDead => currentHealthPoint == 0f;

        protected virtual void OnDestroy()
        {
            OnDeath = null;
            OnHit = null;
            HealthPointChanged = null;
        }
    }
}
