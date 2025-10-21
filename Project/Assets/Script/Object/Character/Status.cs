using System;
using Backend.Util.Data;
using Backend.Util.Presentation;
using UnityEngine;

namespace Backend.Object.Character
{
    public class Status : MonoBehaviour, IDamagable, IModel
    {
        [Header("Data References")]
        [SerializeField] protected StatusData data;

        [Header("Health Point Information")]
        [SerializeField] private float currentHealthPoint;
        [SerializeField] private float maximumHealthPoint;

        protected float HealthPoint;

        protected virtual void Awake()
        {
            currentHealthPoint = data.HealthPoint;
            maximumHealthPoint = data.HealthPoint;
        }

        public virtual void TakeDamage(float damage)
        {
            //TODO: 현재 체력과 최대 체력을 구분해야 하기 때문에 해당 기능을 수정해야 한다.
            HealthPoint -= damage;
            currentHealthPoint -= damage;

            HealthPointChanged?.Invoke(NormalizedHealthPoint);
        }

        public event Action<float> HealthPointChanged;

        private float NormalizedHealthPoint => currentHealthPoint / maximumHealthPoint;
    }
}
