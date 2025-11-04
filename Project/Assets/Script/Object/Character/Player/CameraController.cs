using UnityEngine;

namespace Backend.Object.Character.Player
{
    public class CameraController : MonoBehaviour
    {
        #region SERIALIZABLE PROPERTIES API

        [field: Header("Camera Settings")]
        [field: Tooltip("Upper limits (in degrees) for vertical rotation (along the local x-axis of the instance).\n\n" +
                        "인스턴스의 로컬 X-축에 대한 수직 회전의 상한값(도 단위).")]
        [field: Range(0f, 90f)]
        [field: SerializeField] public float UpperVerticalLimit { get; private set; } = 60f;

        [field: Tooltip("Lower limits (in degrees) for vertical rotation (along the local x-axis of the instance).\n\n" +
                        "인스턴스의 로컬 X-축에 대한 수직 회전의 하한값(도 단위).")]
        [field: Range(0f, 90f)]
        [field: SerializeField] public float LowerVerticalLimit { get; private set; } = 60f;

        [field: Header("Base Controller Settings")]
        [field: Tooltip("This value controls how smoothly the old camera rotation angles will be interpolated toward the new camera rotation angles. " +
                        "Setting this value to '50f' (or above) will result in no smoothing at all. " +
                        "Setting this value to '1f' (or below) will result in very noticeable smoothing. " +
                        "For most situations, a value of '25f' is recommended.\n\n" +
                        "해당 값은 기존 카메라 회전 각도가 새로운 카메라 회전 각도로 얼마나 부드럽게 보간될지를 제어한다. " +
                        "이 값을 '50f'(또는 그 이상)로 설정하면 스무딩이 전혀 적용되지 않는다. " +
                        "이 값을 '1f'(또는 그 이하)로 설정하면 매우 눈에 띄는 부드러움 효과가 발생한다. " +
                        "대부분의 경우 '25f' 값을 권장한다.")]
        [field: Range(1f, 50f)]
        [field: SerializeField] public float SmoothingFactor { get; private set; } = 25f;

        [field: Tooltip("Turning camera speed.\n\n" +
                        "카메라 회전 속도.")]
        [field: SerializeField] public float TurningSpeed { get; private set; } = 250f;

        [field: Tooltip("Whether camera rotation values will be smoothed.\n\n" +
                        "카메라 회전 값이 부드럽게 처리될지 여부.")]
        [field: SerializeField] public bool IsSmoothMode { get; private set; }

        #endregion

        private Camera _camera;

        private Vector2 _angles;
        private Vector2 _cache;

        protected virtual void Awake()
        {
            InputSystem = GetComponent<MouseInputSystem>();

            _camera = GetComponentInChildren<Camera>();

            // Set angle variables to current rotation angles of this transform.
            var angles = transform.localRotation.eulerAngles;
            _angles = new Vector2(angles.x, angles.y);

            // Execute camera rotation code once to calculate facing and upwards direction.
            RotateCamera(0f, 0f);
        }

        protected virtual void Update()
        {
            Rotate();
        }

        /// <summary>
        /// Get user input and handle camera rotation.
        /// This method can be overridden in classes derived from this base class to modify camera behaviour.
        /// </summary>
        protected virtual void Rotate()
        {
            if (InputSystem == null)
            {
                return;
            }

            //Get input values;
            var h = InputSystem.GetHorizontalCameraInput();
            var v = InputSystem.GetVerticalCameraInput();

            RotateCamera(h, v);
        }

        private void RotateCamera(float horizontal, float vertical)
        {
            float x;
            float y;

            if (IsSmoothMode)
            {
                var time = SmoothingFactor * Time.deltaTime;
                x = Mathf.Lerp(_cache.x, horizontal, time);
                y = Mathf.Lerp(_cache.y, vertical, time);

                _cache = new Vector2(x, y);
            }
            else
            {
                _cache = new Vector2(horizontal, vertical);
            }

            // Add input to camera angles.
            x = _angles.x + (_cache.y * TurningSpeed * Time.deltaTime);
            y = _angles.y + (_cache.x * TurningSpeed * Time.deltaTime);

            // Clamp vertical rotation.
            x = Mathf.Clamp(x, -UpperVerticalLimit, LowerVerticalLimit);

            _angles = new Vector2(x, y);

            UpdateRotation();
        }

        /// <summary>
        /// Update camera rotation based on x and y angles.
        /// </summary>
        private void UpdateRotation()
        {
            transform.localRotation = Quaternion.Euler(new Vector3(0f, _angles.y, 0f));

            //Save 'facingDirection' and 'upwardsDirection' for later;
            FacingDirection = transform.forward;
            UpDirection = transform.up;

            transform.localRotation = Quaternion.Euler(new Vector3(_angles.x, _angles.y, 0f));
        }

        /// <summary>
        /// Rotate the camera toward a rotation that points at a world position in the scene.
        /// </summary>
        protected void RotateTowardPosition(Vector3 position, float speed)
        {
            //Calculate target look vector;
            var direction = position - transform.position;

            RotateTowardDirection(direction, speed);
        }

        /// <summary>
        /// Rotate the camera toward a look vector in the scene.
        /// </summary>
        private void RotateTowardDirection(Vector3 direction, float speed)
        {
            // Normalize direction.
            direction.Normalize();

            // Transform target look vector to this transform's local space.
            direction = transform.parent.InverseTransformDirection(direction);

            // Calculate (local) current look vector;.
            var forward = AimingDirection;
            forward = transform.parent.InverseTransformDirection(forward);

            // Calculate x angle difference.
            var a = new Vector3(0f, forward.y, 1f);
            var b = new Vector3(0f, direction.y, 1f);
            var x = a.SignedAngle(b, Vector3.right);

            // Calculate y angle difference.
            forward.y = 0f;
            direction.y = 0f;
            var y = forward.SignedAngle(direction, Vector3.up);

            // Turn angle values into Vector2 variables for better clamping.
            var angles = _angles;
            var difference = new Vector2(x, y);

            // Calculate normalized direction.
            var magnitude = difference.magnitude;
            if (magnitude == 0f)
            {
                return;
            }

            difference /= magnitude;

            // Check for overshooting.
            if (speed * Time.deltaTime > magnitude)
            {
                angles += difference * magnitude;
            }
            else
            {
                angles += difference * (speed * Time.deltaTime);
            }

            // Set new angles.
            y = angles.y;

            // Clamp vertical rotation.
            x = Mathf.Clamp(angles.x, -UpperVerticalLimit, LowerVerticalLimit);

            _angles = new Vector2(x, y);

            UpdateRotation();
        }

        protected Vector2 Angles
        {
            get => _angles;
            set
            {
                _angles = value;

                UpdateRotation();
            }
        }

        public MouseInputSystem InputSystem { get; private set; }

        /// <summary>
        /// The camera's field-of-view(FOV).
        /// </summary>
        public float FieldOfView
        {
            get { return _camera.fieldOfView; }
            set { _camera.fieldOfView = value; }
        }

        /// <returns>
        /// The direction the camera is facing, without any vertical rotation.
        /// This vector should be used for movement-related purposes(e.g., moving forward).
        /// </returns>
        public Vector3 FacingDirection { get; private set; }

        /// <returns>
        /// The 'right' vector of this instance.
        /// </returns>
        public Vector3 StrafeDirection => transform.right;

        /// <returns>
        /// The 'forward' vector of this instance.
        /// This vector points in the direction the camera is "aiming" and could be used for instantiating projectiles or raycasts.
        /// </returns>
        public Vector3 AimingDirection => transform.forward;

        /// <summary>
        /// The 'up' vector of this instance.
        /// </summary>
        public Vector3 UpDirection { get; private set; }
    }
}
