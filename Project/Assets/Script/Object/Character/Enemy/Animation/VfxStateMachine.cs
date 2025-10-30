using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Backend.Object.Character.Enemy.Animation
{
    [CustomAttribute(
        AnimationEvent.EventType.PlayEffect, AnimationEvent.EventType.StopEffect)]
    public class VfxStateMachine : StateMachineBase
    {
        private VisualEffectPlayer _visualEffectPlayer;
        private ParticleSystem _effect;

        public override void InitializeEventHandlers()
        {
            //_eventHandlers.Add(AnimationEvent.EventType.SetEffect, HandleSetEffect);
            _eventHandlers.Add(AnimationEvent.EventType.PlayEffect, HandlePlayEffect);
            _eventHandlers.Add(AnimationEvent.EventType.StopEffect, HandleStopEffect);
        }

        public override void InitializeComponents(Animator animator)
        {
            base.InitializeComponents(animator);
            _visualEffectPlayer = animator.GetComponent<VisualEffectPlayer>();
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
        }

        //private void HandleSetEffect(AnimationEvent e)
        //{
        //    if (_visualEffectPlayer != null)
        //    {
        //        _visualEffectPlayer.SetEffect(e.Index);
        //    }
        //}

        private void HandlePlayEffect(AnimationEvent e)
        {
            if (_visualEffectPlayer != null)
            {
                _effect = _visualEffectPlayer.Play(e.Index, _animator.transform.position + e.Pos, _animator.transform.rotation);
            }
        }

        private void HandleStopEffect(AnimationEvent e)
        {
            if (_visualEffectPlayer != null && _effect != null)
            {
                _visualEffectPlayer.Stop(e.Index, _effect);
            }
        }
    }
}

