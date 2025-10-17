using System;
using System.Collections.Generic;
using Backend.Object.Character.Enemy;
using Backend.Object.Management;
using Backend.Util.Debug;
using UnityEngine;

namespace Backend.Object.Character
{
    [System.Serializable]
    public struct AnimationEvent
    {
        public enum EventType { SetEffect, PlayEffect, PlaySfx, StopSfx, StopEffect, SetWeapon, StartAttack, EndAttack, SetSpeed, SetParry, PlaySkill, MovePos }
        public string Descript;
        public EventType TypeEvent;
        [Range(0f, 1f)] public float NormalizeTime;
        public int Index;
        public float Value;
        public bool IsBool;
    }

    public abstract class StateMachineBase : StateMachineBehaviour
    {
        [Header("Events")]
        public AnimationEvent[] AnimationEvents;

        private bool[] _eventsFired;
        protected Dictionary<AnimationEvent.EventType, Action<AnimationEvent>> _eventHandlers;

        protected Animator _animator;
        protected AudioSource _audioSource;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _animator = animator;
            InitializeComponents(_animator);

            _eventHandlers = new Dictionary<AnimationEvent.EventType, Action<AnimationEvent>>();
            InitializeEventHandlers();

            _eventsFired = new bool[AnimationEvents.Length];
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            float normalizeTime = stateInfo.normalizedTime % 1;

            for (int i = 0; i < AnimationEvents.Length; i++)
            {
                if (!_eventsFired[i] && normalizeTime >= AnimationEvents[i].NormalizeTime)
                {
                    ProcessEvent(AnimationEvents[i]);
                    _eventsFired[i] = true;
                }
            }
        }

        public virtual void InitializeComponents(Animator animator)
        {
            _audioSource = animator.GetComponent<AudioSource>();
        }

        public abstract void InitializeEventHandlers();

        //private void HandlePlaySfx(Animator animator, AnimationEvent e)
        //{
        //    //if (e.Sfx != null && _audioSource != null)
        //    //{
        //    //    _audioSource.PlayOneShot(e.Sfx);
        //    //}
        //    Debugger.LogMessage("PlaySfx");
        //}

        //private void HandleStopSfx(Animator animator, AnimationEvent e)
        //{
        //    Debugger.LogMessage("StopSfx");
        //}

        public void ProcessEvent(AnimationEvent e)
        {
            if (_eventHandlers.TryGetValue(e.TypeEvent, out var handler))
            {
                handler?.Invoke(e);
            }
        }
    }
}
