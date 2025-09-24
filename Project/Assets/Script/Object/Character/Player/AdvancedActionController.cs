using System;
using UnityEngine;

namespace Backend.Object.Character.Player
{
    [RequireComponent(typeof(CharacterKeyboardInput))]
    [RequireComponent(typeof(MovementController))]
    public class AdvancedActionController : MonoBehaviour
    {
        #region SERIALIZABLE FIELD API

        [Header("Target Reference")]
        [Tooltip("Optional camera transform used for calculating movement direction. If assigned, character movement will take camera view into account.\n\n" +
                 "움직임 방향 계산에 사용되는 선택적 카메라 트렌스폼 레퍼런스. 할당된 경우 캐릭터 움직임이 카메라 시점을 고려한다.")]
        public Transform cameraTransform;

        [Header("Physics Settings")]
        [Tooltip("Movement speed.\n\n" +
                 "이동 속도.")]
        public float movementSpeed = 7f;

        [Tooltip("The speed at which a character descends a steep slope.\n\n" +
                 "캐릭터가 가파른 경사면을 내려갈 때의 속도.")]
        public float slidingSpeed = 5f;

        [Tooltip("Acceptable slope angle limit.\n\n" +
                 "허용 가능한 경사각 한계")]
        public float slopeLimit = 80f;

        [Tooltip("How fast the controller can change direction while in the air. Higher values result in more air control.\n\n" +
                 "공중에 있을 때 컨트롤러가 방향을 전환할 수 있는 속도. 더 높은 값은 더 많은 제어력를 가진다.")]
        public float airControlRate = 2f;

        [Tooltip("Amount of downward gravity.\n\n" +
                 "하향 중력의 양.")]
        public float gravity = 30f;

        [Tooltip("Jump speed.\n\n" +
                 "점프 속도.")]
        public float jumpSpeed = 10f;

        [Tooltip("Jump duration variables.\n\n" +
                 "점프 지속 시간")]
        public float jumpDuration = 0.2f;

        [Tooltip("Air friction determines how fast the controller loses its momentum while in the air.\n\n" +
                 "공기 마찰은 컨트롤러가 공중에 있을 때 운동량을 얼마나 빨리 잃는지를 결정한다.")]
        public float airFriction = 0.5f;

        [Tooltip("Ground friction is used instead, if the controller is grounded.\n\n" +
                 "컨트롤러가 접지된 경우 접지 마찰이 대신 사용된다.")]
        public float groundFriction = 100f;

        [Tooltip("If true calculate and apply momentum relative to the controller's transform.\n\n" +
                 "컨트롤러의 변환에 대한 상대적 운동량을 계산하고 적용할지 여부.")]
        public bool useLocalSpace;

        #endregion

        private MovementController _controller;
        private CharacterKeyboardInput _input;
        private CeilingDetector _detector;

        private ControllerState _state = ControllerState.Falling;

        private Vector3 _momentum = Vector3.zero;

        private void Awake()
        {
            _controller = GetComponent<MovementController>();
            _input = GetComponent<CharacterKeyboardInput>();
            _detector = GetComponent<CeilingDetector>();

            if (_input == null)
            {
                Debug.LogWarning("No character input script has been attached to this instance.", gameObject);
            }
        }

        public void FixedUpdate()
        {
            // Check if mover is grounded.
            _controller.CheckForGround();

            // Determine controller state.
            _state = DetermineControllerState();

            // Apply friction and gravity to momentum.
            ApplyPhysicalForces();

            // Check if the player has initiated a jump.
            HandleJumping();

            // Calculate movement velocity.
            var velocity = Vector3.zero;
            if (_state == ControllerState.Grounded)
            {
                velocity = CalculateMovementVelocity();
            }

            // If local momentum is used, transform momentum into world space first.
            var momentum = _momentum;
            if (useLocalSpace)
            {
                momentum = transform.localToWorldMatrix * _momentum;
            }

            // Add current momentum to velocity.
            velocity += momentum;

            // If player is grounded or sliding on a slope, extend mover's sensor range.
            // This enables the player to walk up/downstairs and slopes without losing ground contact.
            _controller.SetExtendRangeToUsing(IsGrounded);

            // Set mover velocity.
            _controller.SetVelocity(velocity);

            // Store velocity for next frame.
            Velocity = velocity;

            // Save controller movement velocity.
            MovementVelocity = CalculateMovementVelocity();

            // Reset jump key booleans.
            _input.WasJumpKeyReleased = false;
            _input.WasJumpKeyPressed = false;

            // Reset ceiling detector, if one is attached to this instance.
            _detector?.Refresh();
        }

        /// <returns>
        /// Movement direction based on player input.
        /// </returns>
        private Vector3 CalculateMovementDirection()
        {
            // If no character input script is attached to this object, return.
            if (_input == null)
            {
                return Vector3.zero;
            }

            var velocity = Vector3.zero;

            // If no camera transform has been assigned, use the character's transform axes to calculate the movement direction.
            if (cameraTransform == null)
            {
                velocity += transform.right * _input.GetHorizontalMovementInput();
                velocity += transform.forward * _input.GetVerticalMovementInput();
            }
            else
            {
                // If a camera transform has been assigned, use the assigned transform's axes for movement direction.
                // Project movement direction so movement stays parallel to the ground.
                var v = Vector3.ProjectOnPlane(cameraTransform.forward, transform.up).normalized;
                var h = Vector3.ProjectOnPlane(cameraTransform.right, transform.up).normalized;
                velocity +=  h * _input.GetHorizontalMovementInput();
                velocity +=  v * _input.GetVerticalMovementInput();
            }

            // If necessary, clamp movement vector to magnitude of '1f'.
            if (velocity.magnitude > 1f)
            {
                velocity.Normalize();
            }

            return velocity;
        }

        /// <returns>
        /// Movement velocity based on player input, controller state, ground normal, etc.
        /// </returns>
        private Vector3 CalculateMovementVelocity()
        {
            // Calculate normalized movement direction and multiply normalized velocity with movement speed.
            var direction = CalculateMovementDirection();
            direction *= movementSpeed;

            return direction;
        }

        /// <summary>
        /// Determine current controller state based on current momentum and whether the controller is grounded (or not).
        /// </summary>
        private ControllerState DetermineControllerState()
        {
            // Check if vertical momentum is pointing upwards.
            var isRising = IsAirborne() && VectorMath.GetDotProduct(ConvertMomentumToWorldSpace(), transform.up) > 0f;

            // Check if controller is sliding.
            var isSliding = _controller.IsGrounded() && IsGroundTooSteep();

            switch (_state)
            {
                case ControllerState.Grounded:
                {
                    if (isRising)
                    {
                        TranslateToAirborne();

                        return ControllerState.Rising;
                    }

                    if (_controller.IsGrounded() == false)
                    {
                        TranslateToAirborne();

                        return ControllerState.Falling;
                    }

                    if (isSliding == false)
                    {
                        return ControllerState.Grounded;
                    }

                    TranslateToAirborne();

                    return ControllerState.Sliding;
                }
                case ControllerState.Sliding:
                {
                    if (isRising)
                    {
                        TranslateToAirborne();

                        return ControllerState.Rising;
                    }

                    if (_controller.IsGrounded() == false)
                    {
                        TranslateToAirborne();

                        return ControllerState.Falling;
                    }

                    if (_controller.IsGrounded() == false || isSliding)
                    {
                        return ControllerState.Sliding;
                    }

                    TranslateToGrounded();

                    return ControllerState.Grounded;
                }
                case ControllerState.Falling:
                {
                    if (isRising)
                    {
                        return ControllerState.Rising;
                    }

                    if (_controller.IsGrounded() == false || isSliding)
                    {
                        return isSliding ? ControllerState.Sliding : ControllerState.Falling;
                    }

                    TranslateToGrounded();

                    return ControllerState.Grounded;
                }
                case ControllerState.Rising:
                {
                    if (isRising == false)
                    {
                        if (_controller.IsGrounded() && isSliding == false)
                        {
                            TranslateToGrounded();

                            return ControllerState.Grounded;
                        }

                        if (isSliding)
                        {
                            return ControllerState.Sliding;
                        }

                        if (_controller.IsGrounded() == false)
                        {
                            return ControllerState.Falling;
                        }
                    }

                    // If a ceiling detector has been attached to this instance, check for ceiling hits.
                    if (_detector == null)
                    {
                        return ControllerState.Rising;
                    }

                    if (_detector.WasDetected == false)
                    {
                        return ControllerState.Rising;
                    }

                    OnCeilingContact();

                    return ControllerState.Falling;
                }
                case ControllerState.Jumping:
                {
                    // Check for jump timeout.
                    if (Time.time - _input.LastJumpPressedTime > jumpDuration)
                    {
                        return ControllerState.Rising;
                    }

                    // Check if jump key was let go.
                    if (_input.WasJumpKeyReleased)
                    {
                        return ControllerState.Rising;
                    }

                    // If a ceiling detector has been attached to this instance, check for ceiling hits.
                    if (_detector == null)
                    {
                        return ControllerState.Jumping;
                    }

                    if (_detector.WasDetected == false)
                    {
                        return ControllerState.Jumping;
                    }

                    OnCeilingContact();

                    return ControllerState.Falling;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Check if player has initiated a jump.
        /// </summary>
        private void HandleJumping()
        {
            if (_state != ControllerState.Grounded)
            {
                return;
            }

            if ((_input.IsJumpKeyPressed == false && _input.WasJumpKeyPressed == false) || _input.IsJumpLocked)
            {
                return;
            }

            TranslateToAirborne();

            Jump();

            _state = ControllerState.Jumping;
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
                v = VectorMath.ExtractDotVector(_momentum, transform.up);
                h = _momentum - v;
            }

            // Add gravity to vertical momentum.
            v -= transform.up * (gravity * Time.deltaTime);

            // Remove any downward force if the controller is grounded.
            if (_state == ControllerState.Grounded && VectorMath.GetDotProduct(v, transform.up) < 0f)
            {
                v = Vector3.zero;
            }

            // Manipulate momentum to steer controller in the air (if controller is not grounded or sliding).
            if (IsGrounded == false)
            {
                var velocity = CalculateMovementVelocity();

                // If controller has received additional momentum from somewhere else.
                if (h.magnitude > movementSpeed)
                {
                    // Prevent unwanted accumulation of speed in the direction of the current momentum.
                    if (VectorMath.GetDotProduct(velocity, h.normalized) > 0f)
                    {
                        velocity = VectorMath.RemoveDotVector(velocity, h.normalized);
                    }

                    // Lower air control slightly with a multiplier to add some 'weight' to any momentum applied to the controller.
                    const float multiplier = 0.25f;
                    h += velocity * (airControlRate * multiplier * Time.deltaTime);
                }
                // If controller has not received additional momentum.
                else
                {
                    // Clamp _horizontal velocity to prevent accumulation of speed.
                    h += velocity * (airControlRate * Time.deltaTime);
                    h = Vector3.ClampMagnitude(h, movementSpeed);
                }
            }

            // Steer controller on slopes.
            if (_state == ControllerState.Sliding)
            {
                // Calculate vector pointing away from slope.
                var direction = Vector3.ProjectOnPlane(_controller.GetGroundNormal(), transform.up).normalized;

                // Calculate movement velocity and remove all velocity that is pointing up the slope.
                var velocity = CalculateMovementVelocity();
                velocity = VectorMath.RemoveDotVector(velocity, direction);

                // Add movement velocity to momentum.
                h += velocity * Time.fixedDeltaTime;
            }

            // Apply friction to horizontal momentum based on whether the controller is grounded;
            var friction = _state == ControllerState.Grounded ? groundFriction : airFriction;
            h = VectorMath.IncrementVectorTowardTargetVector(h, friction, Time.deltaTime, Vector3.zero);

            // Add horizontal and vertical momentum back together.
            _momentum = h + v;

            switch (_state)
            {
                // Additional momentum calculations for sliding.
                case ControllerState.Sliding:
                {
                    // Project the current momentum onto the current ground normal if the controller is sliding down a slope.
                    _momentum = Vector3.ProjectOnPlane(_momentum, _controller.GetGroundNormal());

                    // Remove any upwards momentum when sliding.
                    if (VectorMath.GetDotProduct(_momentum, transform.up) > 0f)
                    {
                        _momentum = VectorMath.RemoveDotVector(_momentum, transform.up);
                    }

                    // Apply additional slide gravity.
                    var direction = Vector3.ProjectOnPlane(-transform.up, _controller.GetGroundNormal()).normalized;
                    _momentum += direction * (slidingSpeed * Time.deltaTime);

                    break;
                }
                // If controller is jumping, override vertical velocity with jump speed.
                case ControllerState.Jumping:
                    _momentum = VectorMath.RemoveDotVector(_momentum, transform.up);
                    _momentum += transform.up * jumpSpeed;
                    break;
                case ControllerState.Grounded:
                case ControllerState.Falling:
                case ControllerState.Rising:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _momentum = ConvertMomentumToLocalSpace();
        }

        /// <summary>
        /// This function is called when the player has initiated a jump.
        /// </summary>
        private void Jump()
        {
            _momentum = ConvertMomentumToWorldSpace();

            // Add jump force to momentum.
            _momentum += transform.up * jumpSpeed;

            // Set jump start time.
            _input.LastJumpPressedTime = Time.time;

            // Lock jump input until jump key is released again.
            _input.IsJumpLocked = true;

            OnJump?.Invoke(_momentum);

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
                var dot = VectorMath.GetDotProduct(momentum.normalized, velocity.normalized);

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
            if (useLocalSpace)
            {
                momentum = transform.localToWorldMatrix * momentum;
            }

            OnLand.Invoke(momentum);
        }

        /// <summary>
        /// This function is called when the controller has collided with a ceiling while jumping or moving upwards.
        /// </summary>
        private void OnCeilingContact()
        {
            _momentum = ConvertMomentumToWorldSpace();

            // Remove all vertical parts of momentum.
            _momentum = VectorMath.RemoveDotVector(_momentum, transform.up);

            _momentum = ConvertMomentumToLocalSpace();
        }

        /// <returns>
        /// True if vertical momentum is above a small threshold. otherwise false.
        /// </returns>
        private bool IsAirborne()
        {
            // Set up threshold to check against.
            // For most applications, a value of '0.001f' is recommended.
            const float limit = 0.001f;

            // Calculate current vertical momentum.
            var momentum = VectorMath.ExtractDotVector(ConvertMomentumToWorldSpace(), transform.up);

            // Return true if vertical momentum is above limit.
            return momentum.magnitude > limit;
        }

        /// <returns>
        /// True if angle between controller and ground normal is too big (greater than slope limit), i.e. ground is too steep. otherwise false.
        /// </returns>
        private bool IsGroundTooSteep()
        {
            if (_controller.IsGrounded() == false)
            {
                return true;
            }

            return Vector3.Angle(_controller.GetGroundNormal(), transform.up) > slopeLimit;
        }

        private Vector3 ConvertMomentumToWorldSpace()
        {
            var momentum = _momentum;

            // If local momentum is used, transform momentum into world coordinates system first.
            if (useLocalSpace)
            {
                momentum = transform.localToWorldMatrix * _momentum;
            }

            return momentum;
        }

        private Vector3 ConvertMomentumToLocalSpace()
        {
            var momentum = _momentum;

            if (useLocalSpace)
            {
                momentum = transform.worldToLocalMatrix * _momentum;
            }

            return momentum;
        }

        /// <returns>
        /// True if controller is grounded (or sliding down a slope).
        /// </returns>
        public bool IsGrounded => _state is ControllerState.Grounded or ControllerState.Sliding;

        /// <returns>
        /// True if controller is sliding.
        /// </returns>
        public bool IsSliding()
        {
            return _state == ControllerState.Sliding;
        }

        public Vector3 Velocity { get; private set; } = Vector3.zero;

        public Vector3 MovementVelocity { get; private set; } = Vector3.zero;

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
            if (useLocalSpace)
            {
                _momentum = transform.worldToLocalMatrix * momentum;
            }
            else
            {
                _momentum = momentum;
            }
        }

        public event Action<Vector3> OnJump;

        public event Action<Vector3> OnLand;

        #region NESTED ENUMERATION API

        public enum ControllerState
        {
            Grounded,
            Sliding,
            Falling,
            Rising,
            Jumping
        }

        #endregion
    }
}
