using System;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace Backend.Object.Character.Player
{
    public class PlayerStatus : Status
    {
        [Header("Default Statue")]
        [SerializeField] private float currentStamina = 100f;
        [SerializeField] private float maxStamina = 100f;
        [SerializeField] private float damagePoint;
        [SerializeField] private float defensePoint;

        [Header("Stamina Settings")]
        [SerializeField] private float regenDelay = 1.0f;
        [SerializeField] private float regenDuration = 3.0f;
        [SerializeField] private float regenAmount = 30.0f;
        [SerializeField] private float cost = 30f;

        private PlayerAnimationController _animationController;
        private AdvancedActionController _actionController;

        private float _regenRate;
        private float _lastUseTime;

        private void Awake()
        {
            _animationController = GetComponent<PlayerAnimationController>();
            _actionController = GetComponent<AdvancedActionController>();
        }

        private void Start()
        {
            _regenRate = regenAmount / regenDuration;
        }

        private void Update()
        {
            if (Time.time - _lastUseTime >= regenDelay && currentStamina < maxStamina)
            {
                currentStamina = Mathf.Min(currentStamina + (_regenRate * Time.deltaTime), maxStamina);
            }
        }

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            var position = new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z);
            Handles.Label(position, $"HP: {HealthPoint:F2}\nSP: {currentStamina:F2}");
        }

#endif

        public override void TakeDamage(float damage)
        {
            base.TakeDamage(damage);

            if (_actionController.IsDamageReactable)
            {
                _animationController.SetAnimationFloat("Damage", damage);
            }
        }

        public void UseStamina()
        {
            currentStamina = Mathf.Max(currentStamina - cost, 0);

            _lastUseTime = Time.time;
        }

        public bool IsUsingStaminaAvailable()
        {
            return currentStamina >= cost;
        }
    }
}
