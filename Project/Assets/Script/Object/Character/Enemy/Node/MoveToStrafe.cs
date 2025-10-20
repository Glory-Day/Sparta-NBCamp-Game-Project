using UnityEngine;

namespace Backend.Object.Character.Enemy.Node
{
    public class MoveToStrafe : ActionNode
    {
        public float StrafeSpeed;
        public float ForwardBackwardSpeed = 1.0f;
        public float MaxDistance;
        public float MinDistance;

        private float _duration;
        private float _direction;
        private float _timer;

        private float _targetMoveX = 0f;
        private float _targetMoveZ = 0f;
        private float _currentMoveX = 0f;
        private float _currentMoveZ = 0f;

        protected override State OnUpdate()
        {
            if (_timer >= _duration)
            {
                _targetMoveX = 0f;
                _targetMoveZ = 0f;

                return State.Success;
            }

            if (agent.MovementController.Target != null)
            {
                float distanceToTarget = agent.MovementController.Distance;

                if (distanceToTarget > MaxDistance)
                {
                    _targetMoveX = 0.5f;
                }
                else if (distanceToTarget < MinDistance)
                {
                    _targetMoveX = -0.5f;
                }
                else
                {
                    _targetMoveX = 0f;
                }
            }

            agent.MovementController.SetLerpRotation(2f);

            Vector3 movement = _direction * StrafeSpeed * Time.deltaTime * agent.MovementController.transform.right;
            Vector3 forwardBackwardMovement = _currentMoveX * ForwardBackwardSpeed * Time.deltaTime * agent.MovementController.transform.forward;
            agent.MovementController.transform.position += movement + forwardBackwardMovement;

            _currentMoveX = Mathf.Lerp(_currentMoveX, _targetMoveX, Time.deltaTime * 5.0f);
            _currentMoveZ = Mathf.Lerp(_currentMoveZ, _targetMoveZ, Time.deltaTime * 5.0f);

            agent.AnimationController.SetAnimationFloat("MoveX", _currentMoveX);
            agent.AnimationController.SetAnimationFloat("MoveZ", _currentMoveZ);

            _timer += Time.deltaTime;

            return State.Running;
        }
        protected override void Start()
        {
            _duration = Random.Range(1.0f, 2.5f);
            _direction = (Random.value > 0.5f) ? 1.0f : -1.0f;

            _targetMoveZ = _direction;

            _currentMoveX = agent.AnimationController.GetAnimationFloat("MoveX");
            _currentMoveZ = agent.AnimationController.GetAnimationFloat("MoveZ");

            _timer = 0f;
        }
        protected override void Stop()
        {
            _targetMoveX = 0f;
            _targetMoveZ = 0f;
        }
    }
}
