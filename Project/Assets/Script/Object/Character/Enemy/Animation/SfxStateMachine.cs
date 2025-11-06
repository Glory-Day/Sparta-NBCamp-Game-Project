using UnityEngine;

namespace Backend.Object.Character.Enemy.Animation
{
    [CustomAttribute(AnimationEvent.EventType.PlaySfx, AnimationEvent.EventType.InitializationSfx)]
    public class SfxStateMachine : StateMachineBase
    {
        private bool _isAttack = false;
        private EffectSoundPlayer _effectSoundPlayer;

        public override void InitializeComponents(Animator animator)
        {
            base.InitializeComponents(animator);
            _effectSoundPlayer = animator.GetComponent<EffectSoundPlayer>();

            if (_effectSoundPlayer == null)
            {
                _effectSoundPlayer = animator.GetComponentInParent<EffectSoundPlayer>();

            }
        }

        public override void InitializeEventHandlers()
        {
            _eventHandlers.Add(AnimationEvent.EventType.PlaySfx, HandlePlaySfx);

        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            float normalizeTime = stateInfo.normalizedTime % 1;

            if (normalizeTime > 0.95f)
            {
                if (!_eventsFired[_eventsFired.Length - 1])
                {
                    return;
                }

                for (int i = 0; i < _eventsFired.Length; i++)
                {
                    _eventsFired[i] = false;
                }
                return;
            }

            for (int i = 0; i < AnimationEvents.Length; i++)
            {
                if (!_eventsFired[i] && normalizeTime >= AnimationEvents[i].NormalizeTime)
                {
                    ProcessEvent(AnimationEvents[i]);
                    _eventsFired[i] = true;
                }
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
        }

        private void HandlePlaySfx(AnimationEvent e)
        {
            _effectSoundPlayer.Play(e.Index);
        }
    }
}
