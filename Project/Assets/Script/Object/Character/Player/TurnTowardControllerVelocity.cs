﻿using UnityEngine;

namespace Backend.Object.Character.Player
{
    public class TurnTowardControllerVelocity : MonoBehaviour
    {
        #region CONSTANT FIELD API
        
        // When the angle between the current direction and the target direction falls below the threshold, the rotation speed gradually slows down (eventually approaching ‘0f’).
        // This adds a smooth effect to the rotation.
        private const float Threshold = 90f;

        #endregion
        
        [Header("Target Reference")]
        public AdvancedActionController controller;
        
        [Header("Controller Settings")]
        [Tooltip("Speed at which this instance turns toward the controller's velocity.\n\n" +
                 "해당 인스턴스가 컨트롤러의 속도를 향해 회전하는 속도.")]
        [SerializeField] private float turningSpeed = 500f;
        
        [Tooltip("When calculating a new direction, ignore the current controller's momentum if true. Otherwise, false.\n\n" +
                 "새로운 방향을 계산할 때 현재 컨트롤러의 운동량을 무시해야 하는지 여부.")]
        [SerializeField] private bool isMomentumIgnored;

        // Current local rotation around the local y-axis of this instance.
        private float _yAxisAngle;
        
        private void OnEnable()
        {
            _yAxisAngle = transform.localEulerAngles.y;
        }
        
        private void Start()
        {
            // Throw warning if no controller has been assigned.
            if (controller != null)
            {
                return;
            }
            
            Debug.LogWarning("No controller script has been assigned to this component.", this);
                
            enabled = false;
        }

        private void LateUpdate()
        {
            // Get controller velocity.
            var velocity = isMomentumIgnored ? controller.MovementVelocity : controller.Velocity;

            // Project velocity onto a plane defined by the upside direction of the parent transform.
            velocity = Vector3.ProjectOnPlane(velocity, transform.parent.up);

            // If the velocity's magnitude is smaller than the threshold, return.
            if (velocity.magnitude < 0.001f)
            {
                return;
            }

            // Normalize velocity direction.
            velocity.Normalize();

            // Get current forward direction vector.
            var forward = transform.forward;

            // Calculate (signed) angle between velocity and forward direction.
            var difference = VectorMath.GetAngle(forward, velocity, transform.parent.up);

            // Calculate angle factor.
            var factor = Mathf.InverseLerp(0f, Threshold, Mathf.Abs(difference));

            // Calculate this frame's step.
            var step = Mathf.Sign(difference) * factor * Time.deltaTime * turningSpeed;

            switch (difference)
            {
                case < 0f when step < difference:
                case > 0f when step > difference:
                    step = difference;
                    break;
            }

            // Add step to current y-axis angle.
            _yAxisAngle += step;

            // Clamp y-axis angle.
            if (_yAxisAngle > 360f)
            {
                _yAxisAngle -= 360f;
            }
            
            if (_yAxisAngle < -360f)
            {
                _yAxisAngle += 360f;
            }

            // Set transform rotation to Euler's angle.
            transform.localRotation = Quaternion.Euler(0f, _yAxisAngle, 0f);
        }
    }
}
