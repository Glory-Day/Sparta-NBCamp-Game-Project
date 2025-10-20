using UnityEngine;

namespace Backend.Object.Character
{
    public class Status : MonoBehaviour, IDamagable
    {
        protected float HealthPoint;

        public virtual void TakeDamage(float damage)
        {
            HealthPoint -= damage;
        }
    }
}
