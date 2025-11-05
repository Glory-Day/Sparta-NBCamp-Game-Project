using UnityEngine;
using Backend.Util.Management;

namespace Backend.Object.Character.Enemy.Animation
{
    [CustomAttribute(AnimationEvent.EventType.PlaySfx, AnimationEvent.EventType.StopSfx)]
    public class SfxStateMachine : StateMachineBase
    {
        private bool _isAttack = false;
        private EffectSoundPlayer _effectSoundPlayer;

        public override void InitializeComponents(Animator animator)
        {
            base.InitializeComponents(animator);
            _effectSoundPlayer = animator.GetComponent<EffectSoundPlayer>();
        }

        public override void InitializeEventHandlers()
        {
            _eventHandlers.Add(AnimationEvent.EventType.PlaySfx, HandlePlaySfx);
            _eventHandlers.Add(AnimationEvent.EventType.StopSfx, HandleStopSfx);
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
        }

        private void HandlePlaySfx(AnimationEvent e)
        {
            _effectSoundPlayer.Play(e.Index);
        }

        private void HandleStopSfx(AnimationEvent e)
        {
            
        }
    }
}
