using System;
using System.Collections;
using System.Collections.Generic;
using Backend.Object.Management;
using Backend.Object.UI;
using Backend.Util.Data;
using Backend.Util.Management;
using Script.Object.Character.Player;
using UnityEngine;


#if UNITY_EDITOR


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



            _damageSender = GetComponentInChildren<DamageSender>();



        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Composer.AdvancedActionController.enabled = true;

            MaximumStaminaPoint = ((PlayerStatusData)data).StaminaPoint;
            CurrentStaminaPoint = MaximumStaminaPoint;

            if (_damageSender == null)
            {
                return;
            }
            _damageSender.PhysicalDamagePoint = data.PhysicalDamage;
            _damageSender.MagicalDamagePoint = data.MagicalDamage;


        }

        private void Start()
        {
            _regenRate = Amount / Duration;
        }

        private void Update()
        {
            if (isDead)
            {
                return;
            }

            if (IsStaminaPointRegenerable && (Time.time - _lastUseTime < Delay || CurrentStaminaPoint >= MaximumStaminaPoint))
            {
                return;
            }

            CurrentStaminaPoint = Mathf.Min(CurrentStaminaPoint + (_regenRate * Time.deltaTime), MaximumStaminaPoint);

            StaminaPointChanged?.Invoke(NormalizedStaminaPoint);
        }

        public override void TakeDamage(float damage, Vector3? position = null)
        {
            if (isDead)
            {
                return;
            }

            base.TakeDamage(damage);

            if (0f >= currentHealthPoint)
            {
                Composer.AnimationController.SetAnimationTrigger("Died");
                Composer.PerspectiveController.Cancel();
                PlayerDie();
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

        private void PlayerDie()
        {
            isDead = true;
            OnDeath?.Invoke();
            Composer.AdvancedActionController.enabled = false;
            Debug.Log("Player Died");
            StartCoroutine(Restart(3f));
        }

        private IEnumerator Restart(float time)
        {
            yield return new WaitForSeconds(time);

            var sceneIndex = DataManager.UserData.SceneIndex;
            var spawnerIndex = DataManager.UserData.SpawnerIndex;
            SceneManager.LoadSceneByIndex(sceneIndex, spawnerIndex);
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

        public void TakeSoul(float soul)
        {
            Data.Soul += soul;
        }

        public Action<float> StaminaPointChanged;

        public bool IsStaminaPointRegenerable { get; set; } = true;

        private float NormalizedStaminaPoint => CurrentStaminaPoint / MaximumStaminaPoint;
    }
}
