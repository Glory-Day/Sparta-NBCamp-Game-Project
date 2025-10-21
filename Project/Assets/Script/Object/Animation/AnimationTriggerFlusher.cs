using UnityEngine;

namespace Backend.Object.Animation
{
    public class AnimationTriggerFlusher : StateMachineBehaviour
    {
        [Header("Animator Settings")]
        [SerializeField] private string triggerName;

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.ResetTrigger(triggerName);
        }
    }
}
