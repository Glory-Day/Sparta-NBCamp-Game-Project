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

        [Header("Damage Point Information")]
        [SerializeField] private float currentDamagePoint;

        [Header("Defense Point Information")]
        [SerializeField] private float currentDefensePoint;

        private PlayerAnimationController _animationController;
        private AdvancedActionController _actionController;

        private DamageSender _damageSender;

        private float _regenRate;
        private float _lastUseTime;

        protected override void Awake()
        {
            base.Awake();

            PointChanged = new Action<int>[7];

            maximumStaminaPoint = ((PlayerStatusData)data).StaminaPoint;
            currentStaminaPoint = maximumStaminaPoint;

            _animationController = GetComponent<PlayerAnimationController>();
            _actionController = GetComponent<AdvancedActionController>();

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
            currentStaminaPoint = Mathf.Max(currentStaminaPoint - cost, 0);

            _lastUseTime = Time.time;

            StaminaPointChanged?.Invoke(NormalizedStaminaPoint);
        }

        public bool IsUsingStaminaAvailable()
        {
            return currentStaminaPoint >= cost;
        }

        public void TestFuction(int index, int point)
        {
            PointChanged[index].Invoke(point);
        }

        public Action<float> StaminaPointChanged;
        //액션 배열
        public Action<int>[] PointChanged;

        private float NormalizedStaminaPoint => currentStaminaPoint / maximumStaminaPoint;
    }
}
