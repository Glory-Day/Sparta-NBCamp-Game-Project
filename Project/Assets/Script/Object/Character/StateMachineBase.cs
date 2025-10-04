using Backend.Object.Character.Enemy;
using Backend.Object.Management;
using Backend.Util.Debug;
using UnityEngine;

namespace Backend.Object.Character
{
    [System.Serializable]
    public struct AnimationEvent
    {
        public enum EventType { SetEffect, PlayEffect, StopEffect, SetWeapon, StartAttack, EndAttack }
        public string Descript;
        public EventType TypeEvent;
        [Range(0f, 1f)] public float NormalizeTime;
        public int Index;
        public AudioClip Sfx;
    }

    public class StateMachineBase : StateMachineBehaviour
    {
        [Header("SFX")]
        public AudioClip Sound;
        public float PlaySoundTime;
        private bool _isPlayedSound = false;

        [Header("VFX")]
        public GameObject Vfx;
        public Vector3 offset = Vector3.zero;
        public float PlayVfxTime;
        private bool _isPlayedVfx = false;
        private GameObject _spawnedVfx;

        [Header("Hit")]
        public float attackEnableTime;
        public float attackDisableTime;
        private bool _isAttackEnabled = false;

        private EnemyCombatController _combatController;
        private AudioSource _audioSource;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (attackEnableTime >= attackDisableTime)
            {
                Debugger.LogError("You Need Change attackEnable < attackDisable");
            }

            if(_combatController == null)
            {
                _combatController = animator.GetComponent<EnemyCombatController>();
            }

            _isPlayedSound = false;
            _isPlayedVfx = false;
            _isAttackEnabled = false;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_combatController != _isAttackEnabled)
            {
                _combatController.EndAttack();
            }

            if(_spawnedVfx != null)
            {
                ObjectPoolManager.Release(_spawnedVfx);
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            float normalizeTime = stateInfo.normalizedTime % 1;

            if (!_isPlayedSound && normalizeTime >= PlaySoundTime)
            {
                if(Sound != null && _audioSource != null)
                {
                    _audioSource.PlayOneShot(Sound);
                }
                _isPlayedSound = true;
            }

            if (!_isPlayedVfx && normalizeTime >= PlayVfxTime)
            {
                if (Vfx != null)
                {
                    _spawnedVfx = ObjectPoolManager.SpawnPoolObject(Vfx, animator.transform.position + offset, Quaternion.identity, null);
                }
                _isPlayedVfx = true;
            }

            if (!_isAttackEnabled && normalizeTime >= attackEnableTime)
            {
                if(_combatController != null)
                {
                    _combatController.StartAttack();
                }
                _isAttackEnabled = true;
            }

            if (_isAttackEnabled && normalizeTime >= attackDisableTime)
            {
                if(_combatController != null)
                {
                    _combatController.EndAttack();
                }
                _isAttackEnabled = false;
            }
        }
    }

}
