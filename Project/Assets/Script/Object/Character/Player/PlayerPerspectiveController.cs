using System;
using System.Collections;
using Backend.Object.Character.Enemy;
using Backend.Util.Debug;
using Backend.Util.Input;
using Script.Object.Character.Player;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace Backend.Object.Character.Player
{
    public class PlayerPerspectiveController : MonoBehaviour
    {
        #region CONSTANT FIELD API

        /// <summary>
        /// When the angle between the current direction and the target direction falls below the threshold, the rotation speed gradually slows down (eventually approaching ‘0f’).
        /// This adds a smooth effect to the rotation.
        /// </summary>
        private const float Threshold = 90f;

        #endregion

        #region SERIALIZABLE PROPERTIES API

        [field: Header("Composition Reference")]
        [field: SerializeField] public PlayerCharacterComposer Composer { get; private set; }

        [field: Header("Controller Settings")]
        [field: Tooltip("Speed at which this instance turns toward the controller's velocity.\n\n" +
                        "해당 인스턴스가 컨트롤러의 속도를 향해 회전하는 속도.")]
        [field: SerializeField] public float TurningSpeed { get; private set; } = 500f;

        [field: Tooltip("When calculating a new direction, ignore the current controller's momentum if true. Otherwise, false.\n\n" +
                        "새로운 방향을 계산할 때 현재 컨트롤러의 운동량을 무시해야 하는지 여부.")]
        [field: SerializeField] public bool IsMomentumIgnored { get; private set; }

        #endregion

        private EnemyCharacterFinder _finder;

        private PlayerControls _controls;

        private EnemyStatus _target;

        // Current local rotation around the local y-axis of this instance.
        private float _yAxisAngle;

        private void Awake()
        {
            _finder = GetComponent<EnemyCharacterFinder>();
        }

        private void OnEnable()
        {
            _yAxisAngle = transform.localEulerAngles.y;

            _controls = new PlayerControls();
            _controls.Enable();
            _controls.Perspective.LockOn.performed += LockOn;
        }

        private void LateUpdate()
        {
            switch (Composer.ThirdPersonCameraController.Mode)
            {
                case PerspectiveMode.ThirdPerson:
                    TurnTowardByVelocity();
                    break;
                case PerspectiveMode.LockOn:
                    TurnTowardByTarget();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnDisable()
        {
            _controls.Perspective.LockOn.performed -= LockOn;
            _controls.Disable();
            _controls = null;
        }

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            const float size = 0.2f;

            var position = transform.position;
            var point = transform.forward * 10f;

            Handles.color = Color.yellow;
            Handles.DrawLine(position, position + point);
            Handles.SphereHandleCap(0, position + point, Quaternion.identity, size, EventType.Repaint);
            Handles.Label(position + point, "Forward");
        }

#endif

        private void LockOn(InputAction.CallbackContext context)
        {
            _target = _finder.FindNearestEnemyStatus();

            var mode = Composer.ThirdPersonCameraController.Mode;
            var target = _target?.transform;
            if (target == null || mode == PerspectiveMode.LockOn)
            {
                Cancel();
            }
            else
            {
                Focus(target);
            }
        }

        private void Focus(Transform target)
        {
            Debugger.LogProgress();

            Composer.ThirdPersonCameraController.InputSystem.Disable();
            Composer.ThirdPersonCameraController.Mode = PerspectiveMode.LockOn;
            Composer.ThirdPersonCameraController.Target = target;

            StartCoroutine(Focusing());

            Composer.AnimationController.SetAnimationBoolean("Is Focusing", true);
        }

        public void Cancel()
        {
            Debugger.LogProgress();

            _finder.Clear();

            StopAllCoroutines();

            Composer.ThirdPersonCameraController.InputSystem.Enable();
            Composer.ThirdPersonCameraController.Mode = PerspectiveMode.ThirdPerson;
            Composer.ThirdPersonCameraController.Target = null;

            Composer.AnimationController.SetAnimationBoolean("Is Focusing", false);
        }

        private IEnumerator Focusing()
        {
            while (true)
            {
                if (_target.IsDead)
                {
                    Cancel();
                }

                yield return null;
            }
        }

        private void TurnTowardByVelocity()
        {
            // Get controller velocity.
            var velocity = IsMomentumIgnored ? Composer.AdvancedActionController.MovementVelocity : Composer.AdvancedActionController.Velocity;

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
            var difference = forward.SignedAngle(velocity, transform.parent.up);

            // Calculate angle factor.
            var factor = Mathf.InverseLerp(0f, Threshold, Mathf.Abs(difference));

            // Calculate this frame's step.
            var step = Mathf.Sign(difference) * factor * Time.deltaTime * TurningSpeed;

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

        private void TurnTowardByTarget()
        {
            var a = Composer.ThirdPersonCameraController.Target.position;
            var b = a - transform.position;
            b.y = 0f;

            if (b != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(b);
            }
        }

        public Vector3 Forward => transform.forward.normalized;
    }
}
