using System;
using Backend.Util.Data;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace Backend.Object.Character.Player
{
    public class PlayerStatus : Status
    {
        [Header("Stamina Point Information")]
        [SerializeField] private float currentStaminaPoint;
        [SerializeField] private float maximumStaminaPoint;

        [Header("Stamina Point Settings")]
        [SerializeField] private float regenDelay = 1.0f;
        [SerializeField] private float regenDuration = 3.0f;
        [SerializeField] private float regenAmount = 30.0f;
        [SerializeField] private float cost = 30f;

        [Header("Damaged Point Settings")]
        [SerializeField] private float lowDamagedPoint;
        [SerializeField] private float highDamagedPoint;

        [Header("Damage Point Information")]
        [SerializeField] private float currentDamagePoint;

        [Header("Defense Point Information")]
        [SerializeField] private float currentDefensePoint;

        private PlayerAnimationController _animationController;
        private AdvancedActionController _actionController;
        private PlayerMovementController _movementController;

        private DamageSender _damageSender;

        private float _regenRate;
        private float _lastUseTime;

        protected override void Awake()
        {
            base.Awake();

            maximumStaminaPoint = ((PlayerStatusData)data).StaminaPoint;
            currentStaminaPoint = maximumStaminaPoint;

            _animationController = GetComponent<PlayerAnimationController>();
            _actionController = GetComponent<AdvancedActionController>();
            _movementController = GetComponent<PlayerMovementController>();

            _damageSender = GetComponentInChildren<DamageSender>();
            _damageSender.PhysicalDamagePoint = data.PhysicalDamage;
            _damageSender.MagicalDamagePoint = data.MagicalDamage;
        }

        private void Start()
        {
            _regenRate = regenAmount / regenDuration;
        }

        private void Update()
        {
            if (Time.time - _lastUseTime >= regenDelay && currentStaminaPoint < maximumStaminaPoint)
            {
                currentStaminaPoint = Mathf.Min(currentStaminaPoint + (_regenRate * Time.deltaTime), maximumStaminaPoint);

                StaminaPointChanged?.Invoke(NormalizedStaminaPoint);
            }
        }

        public override void TakeDamage(float damage, Vector3? position = null)
        {
            base.TakeDamage(damage);

            if (0f >= currentHealthPoint)
            {
                _animationController.SetAnimationTrigger("Dying");

                return;
            }

            _actionController.Direction = (transform.position - position ?? Vector3.zero).normalized;

            if (lowDamagedPoint < damage && damage < highDamagedPoint)
            {
                _animationController.SetAnimationTrigger("Low Damaged");
            }
            else if (highDamagedPoint < damage)
            {
                _animationController.SetAnimationTrigger("High Damaged");
            }
        }

        public void UseStamina()
        {
            currentStaminaPoint = Mathf.Max(currentStaminaPoint - cost, 0);

            _lastUseTime = Time.time;

            StaminaPointChanged?.Invoke(NormalizedStaminaPoint);
        }

        public bool IsUsingStaminaAvailable()
        {
            return currentStaminaPoint >= cost;
        }

        public Action<float> StaminaPointChanged;

        private float NormalizedStaminaPoint => currentStaminaPoint / maximumStaminaPoint;
    }
}
