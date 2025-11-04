using Backend.Util.Debug;
using Script.Object.Character.Player;
using UnityEngine;

namespace Backend.Object.Character.Player
{
    public class PlayerAnimationController : AnimationController
    {
        #region CONSTANT FIELD API

        private const float SmoothingFactor = 40f;

        #endregion

        #region SERIALIZABLE PROPERTIES API

        [field: Header("Composition Reference")]
        [field: SerializeField] public PlayerCharacterComposer Composer { get; private set; }

        [field: Header("Animation Settings")]
        [field: Tooltip("Whether the character is using the strafing blend tree.\n\n" +
                        "캐릭터가 스트레이핑 블렌드 트리를 사용하고 있는지 여부.")]
        [field: SerializeField] public bool UseStrafeAnimation { get; private set; }

        [field: Tooltip("Velocity threshold for landing animation. Animation will only be triggered if downward velocity exceeds this threshold.\n\n" +
                        "착륙 애니메이션의 속도 임계값. 하향 속도가 이 임계값을 초과할 때만 애니메이션이 트리거된다.")]
        [field: SerializeField] public float LandVelocityThreshold { get; private set; } = 5f;

        #endregion

        private DamageSender _damageSender;
        private Transform _transform;

        private Vector3 _cache = Vector3.zero;

        protected override void Awake()
        {
            _damageSender = GetComponentInChildren<DamageSender>();

            Animator = GetComponent<Animator>();
            _transform = Animator.transform;
        }

        private void OnEnable()
        {
            // Connect events to controller events.
            Composer.AdvancedActionController.OnLand += Land;
        }

        private void Update()
        {
            // Get controller velocity.
            var velocity = Composer.AdvancedActionController.Velocity;

            // Split up velocity.
            var h = velocity.Reject(transform.up);
            var v = velocity - h;

            // Smooth horizontal velocity for fluid animation.
            h = Vector3.Lerp(_cache, h, SmoothingFactor * Time.deltaTime);
            _cache = h;

            if (Composer.ThirdPersonCameraController.Mode == PerspectiveMode.LockOn)
            {
                velocity = _transform.InverseTransformVector(h);
                velocity.Normalize();

                SetAnimationFloat("Vertical Speed", velocity.x);
                SetAnimationFloat("Horizontal Speed", velocity.z);
            }
            else
            {
                velocity = h;

                SetAnimationFloat("Horizontal Speed", velocity.magnitude);
            }

            //Pass values to animator;
            SetAnimationBoolean("Is Grounded", Composer.AdvancedActionController.IsGrounded);
        }

        private void OnDisable()
        {
            // Disconnect events to prevent calls to disabled instance.
            Composer.AdvancedActionController.OnLand -= Land;
        }

        private void Land(Vector3 velocity)
        {
            // Only trigger animation if downward velocity exceeds threshold.
            if (Vector3.Dot(velocity, transform.up.normalized) > -LandVelocityThreshold)
            {
                return;
            }

            SetAnimationTrigger("Land");
        }

        #region ANIMATION EVENT API

        private void OnAttackStarted()
        {
            Debugger.LogProgress();

            _damageSender.StartDetection();
        }

        private void OnAttackStopped()
        {
            Debugger.LogProgress();

            _damageSender.StopDetection();
        }

        private void OnButtonBufferedValid()
        {
            Debugger.LogProgress();

            Composer.AdvancedActionController.IsButtonBufferable = true;
        }

        private void OnButtonBufferedInvalid()
        {
            Debugger.LogProgress();

            Composer.AdvancedActionController.IsButtonBufferable = false;
        }

        private void OnColliderEnabled()
        {
            Debugger.LogProgress();

            Composer.AdvancedActionController.EnableDetection();
        }

        private void OnColliderDisabled()
        {
            Debugger.LogProgress();

            Composer.AdvancedActionController.DisableDetection();
        }

        private void OnStateExited()
        {
            Debugger.LogProgress();

            Composer.AdvancedActionController.State = State.Grounded;
        }

        #endregion
    }
}
