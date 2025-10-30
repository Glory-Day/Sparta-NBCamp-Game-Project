using System.Collections;
using System.Collections.Generic;
using GloryDay.BehaviourTree;
using UnityEngine;
using UnityEngine.AI;

namespace Backend.Object.Character.Enemy.Node
{
    public class MoveToBackWard : ActionNode
    {
        public float StepDistance = 10f;
        public float JumpDuration = 0.8f;
        public float JumpHeight = 2.0f;

        public AnimationCurve JumpCurve;
        private readonly int BackStep = Animator.StringToHash("S0107");
        private readonly int Locomotion = Animator.StringToHash("Locomotion");

        private float _elapsedTime;
        private Vector3 _startPos;
        private Vector3 _endPos;
        private bool _isJumping = false;
        protected override void Start()
        {
            _isJumping = false;

            Vector3 direction = agent.MovementController.transform.forward * -1f;
            _startPos = agent.MovementController.transform.position;
            Vector3 targetPos = _startPos + (direction * StepDistance);

            NavMeshHit hitPos;
            if (NavMesh.SamplePosition(targetPos, out hitPos, 1.0f, NavMesh.AllAreas))
            {
                _endPos = hitPos.position;
                _elapsedTime = 0f;
                _isJumping = true;

                agent.NavMeshAgent.enabled = false;
                agent.AnimationController.SetCrossFadeInFixedTime(BackStep, 0.1f);
            }
        }

        protected override void Stop()
        {
            if (agent.NavMeshAgent != null && !agent.NavMeshAgent.enabled)
            {
                agent.NavMeshAgent.enabled = true;
            }

            _isJumping = false;
        }

        protected override State OnUpdate()
        {
            if (!_isJumping)
            {
                return State.Failure;
            }

            _elapsedTime += Time.deltaTime;
            float progress = _elapsedTime / JumpDuration;

            if (progress >= 1.0f)
            {
                agent.MovementController.transform.position = _endPos;
                agent.NavMeshAgent.enabled = true;
                _isJumping = false;

                agent.AnimationController.SetCrossFadeInFixedTime(Locomotion, 0.5f);
                return State.Success;
            }
            else
            {
                Vector3 currentPos = Vector3.Lerp(_startPos, _endPos, progress);
                currentPos.y += JumpCurve.Evaluate(progress) * JumpHeight;

                agent.MovementController.transform.position = currentPos;

                return State.Running;
            }
        }
    }
}
