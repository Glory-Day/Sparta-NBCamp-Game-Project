using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Backend.Object.Character.Player
{
    public class CameraController : MonoBehaviour
    {
        //Upper and lower limits (in degrees) for vertical rotation (along the local x-axis of the instance);
        [Header("Camera Settings")]
        [Range(0f, 90f)]
        [SerializeField] public float upperVerticalLimit = 60f;
        [Range(0f, 90f)]
        [SerializeField] public float lowerVerticalLimit = 60f;
        
        //This value controls how smoothly the old camera rotation angles will be interpolated toward the new camera rotation angles;
        //Setting this value to '50f' (or above) will result in no smoothing at all;
        //Setting this value to '1f' (or below) will result in very noticeable smoothing;
        //For most situations, a value of '25f' is recommended;
        [Header("Base Controller Settings")]
        [Range(1f, 50f)] public float smoothingFactor = 25f;

        //Camera turning speed; 
        public float turningSpeed = 250f;

        //Whether camera rotation values will be smoothed;
        public bool isSmoothMode;

        private Camera _camera;
        private CameraMouseInput _input;
        
        private Vector3 _forward;
        private Vector3 _upward;
        
        private Vector2 _angles;
        private Vector2 _cache;
        
        private void Awake()
        {
            _camera = GetComponent<Camera>();
            _input = GetComponent<CameraMouseInput>();

            if (_input == null)
            {
                Debug.LogWarning("No camera input script has been attached to this instance", gameObject);
            }

            // If no camera component has been attached to this instance, search the transform's children.
            if (_camera == null)
            {
                _camera = GetComponentInChildren<Camera>();
            }

            //Set angle variables to current rotation angles of this transform;
            var angles = transform.localRotation.eulerAngles;
            _angles = new Vector2(angles.x, angles.y);

            //Execute camera rotation code once to calculate facing and upwards direction;
            RotateCamera(0f, 0f);

            SetUp();
        }
        
        protected virtual void SetUp() { }

        private void Update()
        {
            HandleCameraRotation();
        }

        //Get user input and handle camera rotation;
        //This method can be overridden in classes derived from this base class to modify camera behaviour;
        protected virtual void HandleCameraRotation()
        {
            if (_input == null)
            {
                return;
            }

            //Get input values;
            var h = _input.GetHorizontalCameraInput();
            var v = _input.GetVerticalCameraInput();

            RotateCamera(h, v);
        }
        
        private void RotateCamera(float horizontal, float vertical)
        {
            float x;
            float y;
            
            if (isSmoothMode)
            {
                var time = smoothingFactor * Time.deltaTime;
                x = Mathf.Lerp(_cache.x, horizontal, time);
                y = Mathf.Lerp(_cache.y, vertical, time);

                _cache = new Vector2(x, y);
            }
            else
            {
                _cache = new Vector2(horizontal, vertical);
            }

            // Add input to camera angles.
            x = _angles.x + _cache.y * turningSpeed * Time.deltaTime;
            y = _angles.y + _cache.x * turningSpeed * Time.deltaTime;

            // Clamp vertical rotation.
            x = Mathf.Clamp(x, -upperVerticalLimit, lowerVerticalLimit);
            
            _angles = new Vector2(x, y);

            UpdateRotation();
        }

        //Update camera rotation based on x and y angles;
        private void UpdateRotation()
        {
            transform.localRotation = Quaternion.Euler(new Vector3(0f, _angles.y, 0f));

            //Save 'facingDirection' and 'upwardsDirection' for later;
            _forward = transform.forward;
            _upward = transform.up;

            transform.localRotation = Quaternion.Euler(new Vector3(_angles.x, _angles.y, 0f));
        }

        //Set the camera's field-of-view (FOV);
        public void SetFieldOfView(float value)
        {
            if (_camera)
            {
                _camera.fieldOfView = value;
            }
        }

        //Set x and y angle directly;
        protected void SetRotationAngles(float x, float y)
        {
            _angles = new Vector2(x, y);

            UpdateRotation();
        }

        //Rotate the camera toward a rotation that points at a world position in the scene;
        public void RotateTowardPosition(Vector3 position, float speed)
        {
            //Calculate target look vector;
            var direction = position - transform.position;

            RotateTowardDirection(direction, speed);
        }

        //Rotate the camera toward a look vector in the scene;
        private void RotateTowardDirection(Vector3 direction, float speed)
        {
            // Normalize direction.
            direction.Normalize();

            // Transform target look vector to this transform's local space.
            direction = transform.parent.InverseTransformDirection(direction);

            // Calculate (local) current look vector;.
            var forward = GetAimingDirection();
            forward = transform.parent.InverseTransformDirection(forward);

            // Calculate x angle difference.
            var x = VectorMath.GetAngle(new Vector3(0f, forward.y, 1f),
                                                          new Vector3(0f, direction.y, 1f), Vector3.right);

            // Calculate y angle difference.
            forward.y = 0f;
            direction.y = 0f;
            var y = VectorMath.GetAngle(forward, direction, Vector3.up);

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
                angles += difference * speed * Time.deltaTime;
            }

            // Set new angles.
            y = angles.y;
            
            // Clamp vertical rotation.
            x = Mathf.Clamp(angles.x, -upperVerticalLimit, lowerVerticalLimit);
            
            _angles = new Vector2(x, y);

            UpdateRotation();
        }

        public float GetCurrentXAngle()
        {
            return _angles.x;
        }

        public float GetCurrentYAngle()
        {
            return _angles.y;
        }

        //Returns the direction the camera is facing, without any vertical rotation;
        //This vector should be used for movement-related purposes (e.g., moving forward);
        public Vector3 GetFacingDirection()
        {
            return _forward;
        }

        //Returns the 'forward' vector of this gameobject;
        //This vector points in the direction the camera is "aiming" and could be used for instantiating projectiles or raycasts.
        public Vector3 GetAimingDirection()
        {
            return transform.forward;
        }

        // Returns the 'right' vector of this gameobject;
        public Vector3 GetStrafeDirection()
        {
            return transform.right;
        }

        // Returns the 'up' vector of this gameobject;
        public Vector3 GetUpDirection()
        {
            return _upward;
        }
    }
}
