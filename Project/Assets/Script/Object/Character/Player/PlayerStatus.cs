using System;
using Backend.Util.Data;
using UnityEngine;
using Backend.Object.UI;
using Script.Object.Character.Player;


#if UNITY_EDITOR

using UnityEditor;

#endif

namespace Backend.Object.Character.Player
{
    public class PlayerStatus : Status
    {
        #region SERIALIZABLE PROPERTIES API

        [field: Header("Composition References")]
        [field: SerializeField] public PlayerCharacterComposer Composer { get; private set; }

        [field: Header("Data Settings")]
        [field: SerializeField] public PlayerStatusData Data { get; private set; }

        [field: Header("Stamina Point Information")]
        [field: SerializeField] public float CurrentStaminaPoint { get; private set; }
        [field: SerializeField] public float MaximumStaminaPoint { get; private set; }

        [field: Header("Stamina Point Settings")]
        [field: SerializeField] public float Delay { get; private set; } = 1.0f;
        [field: SerializeField] public float Duration { get; private set; } = 3.0f;
        [field: SerializeField] public float Amount { get; private set; } = 30.0f;
        [field: SerializeField] public float[] Costs { get; private set; }

        [field: Header("Damaged Point Settings")]
        [field: SerializeField] public float LowDamagedPoint { get; private set; }
        [field: SerializeField] public float HighDamagedPoint { get; private set; }

        [field: Header("Damage Point Information")]
        [field: SerializeField] public float CurrentDamagePoint { get; private set; }

        [field: Header("Defense Point Information")]
        [field: SerializeField] public float CurrentDefensePoint { get; private set; }

        #endregion

        private DamageSender _damageSender;

        private float _regenRate;
        private float _lastUseTime;

        //인벤토리
        public Inventory inventory;

        protected override void Awake()
        {
            base.Awake();

            PointChanged = new Action<int>[7];

            MaximumStaminaPoint = ((PlayerStatusData)data).StaminaPoint;
            CurrentStaminaPoint = MaximumStaminaPoint;

            _damageSender = GetComponentInChildren<DamageSender>();
            _damageSender.PhysicalDamagePoint = data.PhysicalDamage;
            _damageSender.MagicalDamagePoint = data.MagicalDamage;
        }

        private void Start()
        {
            _regenRate = Amount / Duration;
        }

        private void Update()
        {
            if (IsStaminaPointRegenerable && (Time.time - _lastUseTime < Delay || CurrentStaminaPoint >= MaximumStaminaPoint))
            {
                return;
            }

            CurrentStaminaPoint = Mathf.Min(CurrentStaminaPoint + (_regenRate * Time.deltaTime), MaximumStaminaPoint);

            StaminaPointChanged?.Invoke(NormalizedStaminaPoint);
        }

        public override void TakeDamage(float damage, Vector3? position = null)
        {
            base.TakeDamage(damage);

            if (0f >= currentHealthPoint)
            {
                Composer.AnimationController.SetAnimationTrigger("Dying");

                return;
            }

            Composer.AdvancedActionController.Direction[1] = (transform.position - position ?? Vector3.zero).normalized;

            if (LowDamagedPoint < damage && damage < HighDamagedPoint)
            {
                Composer.AnimationController.SetAnimationTrigger("Low Damaged");
            }
            else if (HighDamagedPoint < damage)
            {
                Composer.AnimationController.SetAnimationTrigger("High Damaged");
            }
        }

        public void UseStamina(int index)
        {
            CurrentStaminaPoint = Mathf.Max(CurrentStaminaPoint - Costs[index], 0);

            _lastUseTime = Time.time;

            StaminaPointChanged?.Invoke(NormalizedStaminaPoint);
        }

        public bool IsUsingStaminaAvailable(int index)
        {
            return CurrentStaminaPoint >= Costs[index];
        }

        public void TestFuction(int index, int point)
        {
            PointChanged[index].Invoke(point);
        }

        public Action<float> StaminaPointChanged;
        //액션 배열
        public Action<int>[] PointChanged;

        public bool IsStaminaPointRegenerable { get; set; } = true;

        private float NormalizedStaminaPoint => CurrentStaminaPoint / MaximumStaminaPoint;
    }
}
