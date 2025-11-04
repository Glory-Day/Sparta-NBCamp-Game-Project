using System;
using Backend.Util.Input;
using Script.Object.Character.Player;
using UnityEngine;
using Debugger = Backend.Util.Debug.Debugger;

namespace Backend.Object.Character.Player
{
    [RequireComponent(typeof(PlayerMovementController))]
    public partial class AdvancedActionController : MonoBehaviour
    {
        #region SERIALIZABLE PROPERTIES API

        [field: Header("Composition References")]
        [field: SerializeField] public PlayerCharacterComposer Composer { get; private set; }

        [field: Header("Target References")]
        [field: Tooltip("Optional camera transform used for calculating movement direction. If assigned, character movement will take camera view into account.\n\n" +
                        "움직임 방향 계산에 사용되는 선택적 카메라 트렌스폼 레퍼런스. 할당된 경우 캐릭터 움직임이 카메라 시점을 고려한다.")]
        [field: SerializeField] public Transform CameraTransform { get; private set; }

        [field: Header("Collider References")]
        [field: SerializeField] public CapsuleCollider Body { get; private set; }
        [field: SerializeField] public CapsuleCollider Detector { get; private set; }

        [field: Header("Physics Settings")]
        [field: Tooltip("Additional speed for any animation or any effective direction.\n\n" +
                        "임의의 애니메이션이나 임의의 효과적인 연출을 위한 추가적인 속도.")]
        [field: SerializeField] public float DeltaSpeed { get; set; } = 1f;

        [field: Tooltip("Movement speed.\n\n" +
                        "이동 속도.")]
        [field: SerializeField] public float MovementSpeed { get; private set; } = 7f;

        [field: Tooltip("The speed at which a character descends a steep slope.\n\n" +
                        "캐릭터가 가파른 경사면을 내려갈 때의 속도.")]
        [field: SerializeField] public float SlidingSpeed { get; private set; } = 5f;

        [field: Tooltip("Acceptable slope angle limit.\n\n" +
                        "허용 가능한 경사각 한계")]
        [field: SerializeField] public float SlopeLimit { get; private set; } = 80f;

        [field: Tooltip("How fast the controller can change direction while in the air. Higher values result in more air control.\n\n" +
                        "공중에 있을 때 컨트롤러가 방향을 전환할 수 있는 속도. 더 높은 값은 더 많은 제어력를 가진다.")]
        [field: SerializeField] public float AirControlRate { get; private set; } = 2f;

        [field: Tooltip("Amount of downward gravity.\n\n" +
                        "하향 중력의 양.")]
        [field: SerializeField] public float Gravity { get; private set; } = 30f;

        [field: Tooltip("Air friction determines how fast the controller loses its momentum while in the air.\n\n" +
                        "공기 마찰은 컨트롤러가 공중에 있을 때 운동량을 얼마나 빨리 잃는지를 결정한다.")]
        [field: SerializeField] public float AirFriction { get; private set; } = 0.5f;

        [field: Tooltip("Ground friction is used instead, if the controller is grounded.\n\n" +
                        "컨트롤러가 접지된 경우 접지 마찰이 대신 사용된다.")]
        [field: SerializeField] public float GroundFriction { get; private set; } = 100f;

        [field: Tooltip("If true calculate and apply momentum relative to the controller's transform.\n\n" +
                        "컨트롤러의 변환에 대한 상대적 운동량을 계산하고 적용할지 여부.")]
        [field: SerializeField] public bool UseLocalSpace { get; private set; }

        #endregion

        private PlayerStatus _status;

        private Vector3 _momentum = Vector3.zero;

        private void Awake()
        {
            _status = GetComponent<PlayerStatus>();

            InitializeStates();
        }

        private void OnEnable()
        {
            Controls = new PlayerControls();
            Controls.Enable();
            Controls.Movement.Move.started += OnMoveStarted;
            Controls.Movement.Move.performed += OnMovePerformed;
            Controls.Movement.Move.canceled += OnMoveCanceled;
            Controls.Movement.Roll.performed += OnRollPerformed;
            Controls.Movement.Attack.performed += OnAttackPerformed;
        }

        private void Start()
        {
            Forward = Composer.PerspectiveController.Forward;

            MovementSpeed = _status.Data.Speed;
        }

        private void Update()
        {
            var isThirdPersonMode = Composer.ThirdPersonCameraController.Mode == PerspectiveMode.ThirdPerson;
            if (isThirdPersonMode && Controls.Movement.Run.IsPressed())
            {
                _status.IsStaminaPointRegenerable = false;

                MovementSpeed = _status.Data.RunningSpeed;
                Composer.AnimationController.SetAnimationBoolean("Is Running", true);

                _status.UseStamina(2);
            }
            else
            {
                _status.IsStaminaPointRegenerable = true;

                MovementSpeed = State == State.Rolling ? _status.Data.RunningSpeed : _status.Data.Speed;
                Composer.AnimationController.SetAnimationBoolean("Is Running", false);
            }
        }

        private void FixedUpdate()
        {
            // Check if mover is grounded.
            Composer.MovementController.Check();

            // Determine controller state.
            State = DetermineState();

            // Apply friction and gravity to momentum.
            ApplyPhysicalForces();

            // Calculate movement velocity.
            var velocity = Vector3.zero;
            if (State is State.Grounded or State.Rolling or State.Attacking)
            {
                velocity = CalculateMovementVelocity();
            }

            // If local momentum is used, transform momentum into world space first.
            var momentum = _momentum;
            if (UseLocalSpace)
            {
                momentum = transform.localToWorldMatrix * _momentum;
            }

            // Add current momentum to velocity.
            velocity += momentum;

            // If player is grounded or sliding on a slope, extend mover's sensor range.
            // This enables the player to walk up/downstairs and slopes without losing ground contact.
            Composer.MovementController.UseExtendedRange = IsGrounded;

            // Set mover velocity.
            Composer.MovementController.Velocity = velocity;

            // Store velocity for next frame.
            Velocity = velocity;

            // Save controller movement velocity.
            MovementVelocity = CalculateMovementVelocity();

#if UNITY_EDITOR

            if (Composer.MovementController.Velocity.magnitude > 1f)
            {
                _velocity = Composer.MovementController.Velocity;
            }

#endif
        }

        private void OnDisable()
        {
            Controls.Movement.Move.started -= OnMoveStarted;
            Controls.Movement.Move.performed -= OnMovePerformed;
            Controls.Movement.Move.canceled -= OnMoveCanceled;
            Controls.Movement.Roll.performed -= OnRollPerformed;
            Controls.Movement.Attack.performed -= OnAttackPerformed;
            Controls.Disable();
            Controls = null;
        }

        /// <returns>
        /// Movement direction based on player input.
        /// </returns>
        private Vector3 CalculateMovementDirection()
        {
            if (Controls == null)
            {
                return Vector3.zero;
            }

            var direction = State == State.Grounded ? Direction[0] : Direction[1];

            // If a camera transform has been assigned, use the assigned transform's axes for movement direction.
            // Project movement direction so movement stays parallel to the ground.
            var isLockedOn = Composer.ThirdPersonCameraController.Mode == PerspectiveMode.LockOn;
            if (isLockedOn)
            {
                var target = Composer.ThirdPersonCameraController.Target;
                var forward = target.position - transform.position;
                forward.y = 0f;
                forward.Normalize();

                var right = Vector3.Cross(Vector3.up, forward).normalized;
                direction = ((right * direction.x) + (forward * direction.z)).normalized;
            }
            else
            {
                var v = Vector3.ProjectOnPlane(CameraTransform.forward, transform.up).normalized;
                var h = Vector3.ProjectOnPlane(CameraTransform.right, transform.up).normalized;
                direction = (h * direction.x) + (v * direction.z);
            }

            // If necessary, clamp movement vector to magnitude of '1f'.
            if (direction.magnitude > 1f)
            {
                direction.Normalize();
            }

            return direction;
        }

        /// <returns>
        /// Movement velocity based on player input, controller state, ground normal, etc.
        /// </returns>
        private Vector3 CalculateMovementVelocity()
        {
            // Calculate normalized movement direction and multiply normalized velocity with movement speed.
            var direction = CalculateMovementDirection();
            direction *= MovementSpeed * DeltaSpeed;

            return direction;
        }

        /// <summary>
        /// Apply friction to both vertical and horizontal momentum based on friction and gravity.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void ApplyPhysicalForces()
        {
            _momentum = ConvertMomentumToWorldSpace();

            // Set vertical and horizontal momentum.
            var v = Vector3.zero;
            var h = Vector3.zero;

            // Split momentum into vertical and horizontal components.
            if (_momentum != Vector3.zero)
            {
                v = _momentum.Project(transform.up);
                h = _momentum - v;
            }

            // Add gravity to vertical momentum.
            v -= transform.up * (Gravity * Time.deltaTime);

            // Remove any downward force if the controller is grounded.
            if (State is State.Grounded or State.Rolling or State.Attacking && Vector3.Dot(v, transform.up.normalized) < 0f)
            {
                v = Vector3.zero;
            }

            // Manipulate momentum to steer controller in the air (if controller is not grounded or sliding).
            if (IsGrounded == false)
            {
                var velocity = CalculateMovementVelocity();

                // If controller has received additional momentum from somewhere else.
                if (h.magnitude > MovementSpeed)
                {
                    // Prevent unwanted accumulation of speed in the direction of the current momentum.
                    if (Vector3.Dot(velocity, h.normalized) > 0f)
                    {
                        velocity = velocity.Reject(h.normalized);
                    }

                    // Lower air control slightly with a multiplier to add some 'weight' to any momentum applied to the controller.
                    const float multiplier = 0.25f;
                    h += velocity * (AirControlRate * multiplier * Time.deltaTime);
                }
                // If controller has not received additional momentum.
                else
                {
                    // Clamp _horizontal velocity to prevent accumulation of speed.
                    h += velocity * (AirControlRate * Time.deltaTime);
                    h = Vector3.ClampMagnitude(h, MovementSpeed);
                }
            }

            // Steer controller on slopes.
            if (State == State.Sliding)
            {
                // Calculate vector pointing away from slope.
                var direction = Vector3.ProjectOnPlane(Composer.MovementController.GetGroundNormal(), transform.up).normalized;

                // Calculate movement velocity and remove all velocity that is pointing up the slope.
                var velocity = CalculateMovementVelocity();
                velocity = velocity.Reject(direction);

                // Add movement velocity to momentum.
                h += velocity * Time.fixedDeltaTime;
            }

            // Apply friction to horizontal momentum based on whether the controller is grounded;
            var friction = State is State.Grounded or State.Rolling or State.Attacking ? GroundFriction : AirFriction;
            h = Vector3.MoveTowards(h, Vector3.zero, friction * Time.deltaTime);

            // Add horizontal and vertical momentum back together.
            _momentum = h + v;

            switch (State)
            {
                // Additional momentum calculations for sliding.
                case State.Sliding:
                {
                    // Project the current momentum onto the current ground normal if the controller is sliding down a slope.
                    _momentum = Vector3.ProjectOnPlane(_momentum, Composer.MovementController.GetGroundNormal());

                    // Remove any upwards momentum when sliding.
                    if (Vector3.Dot(_momentum, transform.up.normalized) > 0f)
                    {
                        _momentum = _momentum.Reject(transform.up);
                    }

                    // Apply additional slide gravity.
                    var direction = Vector3.ProjectOnPlane(-transform.up, Composer.MovementController.GetGroundNormal()).normalized;
                    _momentum += direction * (SlidingSpeed * Time.deltaTime);

                    break;
                }
                case State.Grounded:
                case State.Falling:
                case State.Rising:
                case State.Rolling:
                case State.Attacking:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _momentum = ConvertMomentumToLocalSpace();
        }

        /// <summary>
        /// This function is called when the controller has lost ground contact. (i.e. is either falling or rising, or generally in the air)
        /// </summary>
        private void TranslateToAirborne()
        {
            _momentum = ConvertMomentumToWorldSpace();

            // Get current movement velocity.
            var velocity = MovementVelocity;

            // Check if the controller has both momentum and a current movement velocity.
            if (velocity.sqrMagnitude >= 0f && _momentum.sqrMagnitude > 0f)
            {
                // Project momentum onto movement direction.
                var momentum = Vector3.Project(_momentum, velocity.normalized);

                // Calculate dot product to determine whether momentum and movement are aligned.
                var dot = Vector3.Dot(momentum.normalized, velocity.normalized);

                // If current momentum is already pointing in the same direction as movement velocity,
                // don't add further momentum (or limit movement velocity) to prevent unwanted speed accumulation.
                if (momentum.sqrMagnitude >= velocity.sqrMagnitude && dot > 0f)
                {
                    velocity = Vector3.zero;
                }
                else if (dot > 0f)
                {
                    velocity -= momentum;
                }
            }

            // Add movement velocity to momentum.
            _momentum += velocity;

            _momentum = ConvertMomentumToLocalSpace();
        }

        /// <summary>
        /// This function is called when the controller has landed on a surface after being in the air.
        /// </summary>
        private void TranslateToGrounded()
        {
            if (OnLand == null)
            {
                return;
            }

            var momentum = _momentum;

            // If local momentum is used, transform momentum into world coordinates first.
            if (UseLocalSpace)
            {
                momentum = transform.localToWorldMatrix * momentum;
            }

            OnLand.Invoke(momentum);
        }

        /// <summary>
        /// This function is called when the controller has collided with a ceiling while jumping or moving upwards.
        /// </summary>
        private void ContractCeiling()
        {
            _momentum = ConvertMomentumToWorldSpace();

            // Remove all vertical parts of momentum.
            _momentum = _momentum.Reject(transform.up);

            _momentum = ConvertMomentumToLocalSpace();
        }

        private Vector3 ConvertMomentumToWorldSpace()
        {
            var momentum = _momentum;

            // If local momentum is used, transform momentum into world coordinates system first.
            if (UseLocalSpace)
            {
                momentum = transform.localToWorldMatrix * _momentum;
            }

            return momentum;
        }

        private Vector3 ConvertMomentumToLocalSpace()
        {
            var momentum = _momentum;

            if (UseLocalSpace)
            {
                momentum = transform.worldToLocalMatrix * _momentum;
            }

            return momentum;
        }

        /// <summary>
        /// Add momentum to controller.
        /// </summary>
        public void AddMomentum(Vector3 momentum)
        {
            _momentum = ConvertMomentumToWorldSpace();

            _momentum += momentum;

            _momentum = ConvertMomentumToLocalSpace();
        }

        /// <param name="momentum">Controller momentum directly.</param>
        public void SetMomentum(Vector3 momentum)
        {
            if (UseLocalSpace)
            {
                _momentum = transform.worldToLocalMatrix * momentum;
            }
            else
            {
                _momentum = momentum;
            }
        }

        public void EnableDetection()
        {
            Debugger.LogProgress();

#if UNITY_EDITOR

            _color = Color.blue;

#endif

            Detector.enabled = true;
        }

        public void DisableDetection()
        {
            Debugger.LogProgress();

#if UNITY_EDITOR

            _color = Color.red;

#endif

            Detector.enabled = false;
        }

        public Vector3 Velocity { get; private set; } = Vector3.zero;

        public Vector3 MovementVelocity { get; private set; } = Vector3.zero;
    }
}
