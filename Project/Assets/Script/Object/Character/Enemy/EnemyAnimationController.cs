using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Backend.Object.Character.Enemy.Boss
{
    public class EnemyAnimationController : AnimationController
    {
        protected override void Awake()
        {
            base.Awake();
        }

        public bool IsCurrentState(string stateName, int layer)
        {
            return Animator.GetCurrentAnimatorStateInfo(layer).IsName(stateName);
        }

        public bool IsTransition(int layer)
        {
            return Animator.IsInTransition(layer);
        }

        public float GetNextAnimationLength(int layer)
        {
            var information = Animator.GetNextAnimatorStateInfo(layer);
            return information.length;
        }

        /// <summary>
        ///  Animation Transition -> Get Next Animation Length, Animation Playing -> Get Now Animation Length
        /// </summary>
        public float GetAnimationClipLength(int layer) //태그
        {
            if (Animator.IsInTransition(layer))
            {
                AnimatorStateInfo nextStateInfo = Animator.GetNextAnimatorStateInfo(layer);
                return nextStateInfo.length;
            }
            else
            {
                AnimatorStateInfo currentStateInfo = Animator.GetCurrentAnimatorStateInfo(layer);
                return currentStateInfo.length;
            }
        }

        /// <summary>
        ///  Animation Transition -> Get Next Animation NormalizedTime, Animation Playing -> Get Now Animation NormalizedTime
        /// </summary>
        public float GetAnimationNormalize(int layer)
        {
            if (Animator.IsInTransition(layer))
            {
                AnimatorStateInfo nextStateInfo = Animator.GetNextAnimatorStateInfo(layer);
                return nextStateInfo.normalizedTime;
            }
            else
            {
                AnimatorStateInfo currentStateInfo = Animator.GetCurrentAnimatorStateInfo(layer);
                return currentStateInfo.normalizedTime;
            }
        }

        public bool IsPlayingCheckName(string stateName, int layer)
        {
            if (Animator.IsInTransition(layer))
            {
                AnimatorStateInfo nextStateInfo = Animator.GetNextAnimatorStateInfo(layer);
                return nextStateInfo.IsName(stateName);
            }
            else
            {
                AnimatorStateInfo currentStateInfo = Animator.GetCurrentAnimatorStateInfo(layer);
                return currentStateInfo.IsName(stateName);
            }
        }
    }
}
