using UnityEngine;

namespace Backend.Object.Character.Player
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class MovementController : MonoBehaviour
    {
        #region CONSTANT FIELD API

        private const float RadiusModifier = 0.8f;

        #endregion
        
        [Header("Movement Settings")] [Range(0f, 1f)]
        [SerializeField] private float stepHeightRatio = 0.25f;

        [Header("Collider Settings")]
        [SerializeField] private float height = 2f;
        [SerializeField] private float thickness = 1f;
        [SerializeField] private Vector3 offset = Vector3.zero;
        
        [Header("Debug Settings")]
        [SerializeField] private bool isDebugMode;
        
        [Header("Detection Settings")]
        [SerializeField] public Sensor.CastMethodMode mode = Sensor.CastMethodMode.SingleRay;
        [HideInInspector] public int rows = 1;
        [HideInInspector] public int count = 6;
        [HideInInspector] public bool isOffset;

        [HideInInspector] public Vector3[] multipleRayPositions;
        
        private Collider _collider;
        private CapsuleCollider _capsuleCollider;
        private Rigidbody _rigidbody;
        private Sensor _sensor;
        
        private bool _isGrounded;
        
        private bool _useExtendedRange = true;
        private float _extendedRange;

        private int _layer;
        
        // Current upwards (or downwards) velocity necessary to keep the correct distance to the ground.
        private Vector3 _adjustmentVelocity = Vector3.zero;
        
        private void Awake()
        {
            SetUp();
            
            _sensor = new Sensor(transform, _collider);
            
            RecalculateColliderDimensions();
            RecalibrateSensor();
        }
        
        private void LateUpdate()
        {
            if (isDebugMode)
            {
                _sensor.DrawDebug();
            }
        }

        private void Reset()
        {
            SetUp();
        }

#if UNITY_EDITOR
        
        private void OnValidate()
        {
            // Recalculate collider dimensions.
            if (gameObject.activeInHierarchy)
            {
                RecalculateColliderDimensions();
            }

            // Recalculate raycast array preview positions.
            if (mode == Sensor.CastMethodMode.MultipleRay)
            {
                multipleRayPositions = Sensor.GetRaycastOriginPositions(rows, 1f, count, isOffset);
            }
        }
        
#endif
        
        private void SetUp()
        {
            _collider = GetComponent<Collider>();

            // If no collider is attached to this instance, add a collider.
            if (_collider == null)
            {
                transform.gameObject.AddComponent<CapsuleCollider>();
                _collider = GetComponent<Collider>();
            }

            _rigidbody = GetComponent<Rigidbody>();

            // If no rigidbody is attached to this instance, add a rigidbody.
            if (_rigidbody == null)
            {
                transform.gameObject.AddComponent<Rigidbody>();
                _rigidbody = GetComponent<Rigidbody>();
            }
            
            _capsuleCollider = GetComponent<CapsuleCollider>();

            // Freeze rigidbody rotation and disable rigidbody gravity.
            _rigidbody.freezeRotation = true;
            _rigidbody.useGravity = false;
        }
        
        

        //Recalculate collider height/width/thickness;
        private void RecalculateColliderDimensions()
        {
            // Check if a collider is attached to this instance.
            if (_collider == null)
            {
                // Try to get a reference to the attached collider by calling set up.
                SetUp();
            }

            if (_capsuleCollider)
            {
                _capsuleCollider.height = height;
                _capsuleCollider.center = offset * height;
                _capsuleCollider.radius = thickness / 2f;

                _capsuleCollider.center += new Vector3(0f, stepHeightRatio * _capsuleCollider.height / 2f, 0f);
                _capsuleCollider.height *= 1f - stepHeightRatio;

                if (_capsuleCollider.height / 2f < _capsuleCollider.radius)
                {
                    _capsuleCollider.radius = _capsuleCollider.height / 2f;
                }
            }

            // Recalibrate sensor variables to fit new collider dimensions.
            if (_sensor != null)
            {
                RecalibrateSensor();
            }
        }

        // Recalibrate sensor variables;
        private void RecalibrateSensor()
        {
            //Set sensor ray origin and direction.
            _sensor.SetCastOrigin(GetColliderCenter());
            _sensor.SetCastDirection(Sensor.Direction.Down);

            // Calculate sensor layer mask.
            RecalculateSensorLayerMask();

            // Set sensor cast type;
            _sensor.CastMode = mode;

            // Calculate sensor radius/width.
            var radius = thickness / 2f * RadiusModifier;

            // Multiply all sensor lengths with distance factor to compensate for floating point errors.
            const float factor = 0.001f;

            // Fit collider height to sensor radius.
            if (_capsuleCollider)
            {
                radius = Mathf.Clamp(radius, factor, _capsuleCollider.height / 2f * (1f - factor));
            }

            // Set sensor radius.
            _sensor.Radius = radius * transform.localScale.x;

            // Calculate and set sensor length.
            var length = 0f;
            length += height * (1f - stepHeightRatio) * 0.5f;
            length += height * stepHeightRatio;
            _extendedRange = length * (1f + factor) * transform.localScale.x;
            _sensor.MaximumDistance = length * transform.localScale.x;

            // Set sensor array variables.
            _sensor.Option.Rows = rows;
            _sensor.Option.Count = count;
            _sensor.Option.IsOffset = isOffset;
            _sensor.IsDebugMode = isDebugMode;

            // Set sensor spherecast variables.
            _sensor.UseRealisticDistance = true;
            _sensor.UseRealisticSurfaceNormal = true;

            // Recalibrate sensor to the new values.
            _sensor.RecalibrateRaycastOriginPositions();
        }

        // Recalculate sensor layer mask based on current physics settings;
        private void RecalculateSensorLayerMask()
        {
            var layerMask = 0;
            var layer = gameObject.layer;

            // Calculate layer mask.
            for (var i = 0; i < 32; i++)
            {
                if (Physics.GetIgnoreLayerCollision(layer, i) == false)
                {
                    layerMask |= 1 << i;
                }
            }

            // Make sure that the calculated layer mask does not include the 'Ignore Raycast' layer.
            if (layerMask == (layerMask | (1 << LayerMask.NameToLayer("Ignore Raycast"))))
            {
                layerMask ^= 1 << LayerMask.NameToLayer("Ignore Raycast");
            }

            // Set sensor layer mask.
            _sensor.LayerMask = layerMask;

            // Save current layer.
            _layer = layer;
        }

        // Returns the collider's center in world coordinates;
        private Vector3 GetColliderCenter()
        {
            if (_collider == null)
            {
                SetUp();
            }

            return _collider.bounds.center;
        }

        // Check if mover is grounded;
        //Store all relevant collision information for later;
        //Calculate necessary adjustment velocity to keep the correct distance to the ground;
        private void Check()
        {
            // Reset ground adjustment velocity.
            _adjustmentVelocity = Vector3.zero;

            // Set sensor length.
            if (_useExtendedRange)
            {
                _sensor.MaximumDistance = _extendedRange + height * transform.localScale.x * stepHeightRatio;
            }
            else
            {
                _sensor.MaximumDistance = _extendedRange;
            }

            _sensor.Cast();

            // If sensor has not detected anything, set flags and return.
            if (_sensor.Hit.IsDetected == false)
            {
                _isGrounded = false;
                
                return;
            }

            // Set flags for ground detection.
            _isGrounded = true;

            // Get distance that sensor ray reached.
            var distance = _sensor.Hit.Distance;

            // Calculate how much mover needs to be moved up or down.
            var length = height * transform.localScale.x * (1f - stepHeightRatio) * 0.5f;
            var middle = length + height * transform.localScale.x * stepHeightRatio;
            var difference = middle - distance;

            // Set new ground adjustment velocity for the next frame.
            _adjustmentVelocity = transform.up * (difference / Time.fixedDeltaTime);
        }

        //Check if mover is grounded;
        public void CheckForGround()
        {
            // Check if object layer has been changed since last frame.
            // If so, recalculate sensor layer mask.
            if (_layer != gameObject.layer)
            {
                RecalculateSensorLayerMask();
            }

            Check();
        }

        //Set mover velocity;
        public void SetVelocity(Vector3 velocity)
        {
            _rigidbody.velocity = velocity + _adjustmentVelocity;
        }

        //Returns 'true' if mover is touching ground and the angle between hte 'up' vector and ground normal is not too steep (e.g., angle < slope_limit);
        public bool IsGrounded()
        {
            return _isGrounded;
        }
        
        public void SetExtendRangeToUsing(bool isExtended)
        {
            _useExtendedRange = isExtended;
        }
        
        public void SetColliderHeight(float value)
        {
            height = value;
            
            RecalculateColliderDimensions();
        }
        
        public void SetColliderThickness(float value)
        {
            if (value < 0f)
            {
                value = 0f;
            }

            thickness = value;
            
            RecalculateColliderDimensions();
        }
        
        public void SetStepHeightRatio(float value)
        {
            value = Mathf.Clamp(value, 0f, 1f);
            
            stepHeightRatio = value;
            
            RecalculateColliderDimensions();
        }

        public Vector3 GetGroundNormal()
        {
            return _sensor.Hit.Normal;
        }

        public Vector3 GetGroundPoint()
        {
            return _sensor.Hit.Position;
        }

        public Collider GetGroundCollider()
        {
            return _sensor.Hit.Collider;
        }
    }
}
