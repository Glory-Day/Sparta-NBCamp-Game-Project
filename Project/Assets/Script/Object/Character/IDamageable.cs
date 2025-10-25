using UnityEngine;

namespace Backend.Object.Character
{
    public interface IDamageable
    {
        void TakeDamage(float damage, Vector3? position = null);
    }
}
