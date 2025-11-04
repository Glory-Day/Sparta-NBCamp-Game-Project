using System;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace Backend.Object.Character.Player
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerMovementController : MovementController
    {
        #region CONSTANT FIELD API

        private const float RadiusModifier = 0.8f;

        #endregion

        #region SERIALIZABLE PROPERTIES API

        [field: Header("Movement Settings")]
        [field: Range(0f, 1f)]
        [field: SerializeField] public float StepHeightRatio { get; private set; } = 0.25f;

        [field: Header("Collider Settings")]
        [field: SerializeField] public CapsuleCollider Collider { get; private set; }
        [field: SerializeField] public float Height { get; private set; } = 2f;
        [field: SerializeField] public float Thickness { get; private set; } = 1f;
        [field: SerializeField] public Vector3 Offset { get; private set; } = Vector3.zero;

#if UNITY_EDITOR

        [field: Header("Debug Settings")]
        [field: SerializeField] public bool IsDebugMode { get; set; }

#endif

        [field: Header("Detection Settings")]
        [field: SerializeField] public SensorMode Mode { get; set; } = SensorMode.SingleRay;

        public int Rows { get; set; } = 1;
        public int Count { get; set; } = 6;
        public bool IsOffset { get; set; }

        public Vector3[] MultipleRayPositions { get; private set; }

        #endregion

        private Sensor _sensor;

        private bool _useExtendedRange = true;
        private float _extendedRange;

        private int _layer;

#if UNITY_EDITOR

        private Color _color = Color.green;

#endif

        // Current upwards (or downwards) velocity necessary to keep the correct distance to the ground.
        private Vector3 _adjustmentVelocity = Vector3.zero;

        protected override void Awake()
        {
            base.Awake();

            SetUp();

            _sensor = new Sensor(transform, Collider);

            RecalculateColliderDimensions();
            RecalibrateSensor();
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
            if (Mode == SensorMode.MultipleRay)
            {
                MultipleRayPositions = Sensor.GetRaycastOriginPositions(Rows, 1f, Count, IsOffset);
            }
        }

        private void OnDrawGizmos()
        {
            DrawSensor();
            DrawPosition();
        }

        private void DrawSensor()
        {
            const float radius = 0.04f;

#if UNITY_EDITOR

            if (_sensor == null || _sensor.IsDetected == false || IsDebugMode == false)
            {
                return;
            }

#else

            if (_sensor == null || _sensor.IsDetected == false)
            {
                return;
            }

#endif


            var position = Vector3.zero;
            var distance = Vector3.zero;

            switch (Mode)
            {
                case SensorMode.SingleRay:
                {
                    position = _sensor.Position;
                    distance = _sensor.Normal;

                    Handles.color = Color.red;
                    Handles.DrawLine(position, position + distance);
                    Handles.SphereHandleCap(0, position, Quaternion.identity, radius * 2f, EventType.Repaint);
                    Handles.SphereHandleCap(0, position + distance, Quaternion.identity, radius * 2f, EventType.Repaint);

                    break;
                }
                case SensorMode.MultipleRay:
                {
                    for (var i = 0; i < _sensor.Hits.Count; i++)
                    {
                        position = _sensor.Hits[i].Position;
                        distance = _sensor.Hits[i].Normal;

                        Handles.color = Color.red;
                        Handles.DrawLine(position, position + distance);
                        Handles.SphereHandleCap(0, position, Quaternion.identity, radius * 2f, EventType.Repaint);
                        Handles.SphereHandleCap(0, position + distance, Quaternion.identity, radius * 2f, EventType.Repaint);
                    }

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DrawPosition()
        {
            if (_sensor == null || _sensor.IsDetected == false || IsDebugMode == false)
            {
                return;
            }

            const float size = 0.2f;

            var position = _sensor.Position;
            var distance = _sensor.Normal;

            Handles.color = Color.green;
            Handles.DrawLine(position + (Vector3.up * size), position - (Vector3.up * size));
            Handles.DrawLine(position + (Vector3.right * size), position - (Vector3.right * size));
            Handles.DrawLine(position + (Vector3.forward * size), position - (Vector3.forward * size));
        }

#endif

        private void SetUp()
        {
            // Freeze rigidbody rotation and disable rigidbody gravity.
            Rigidbody.freezeRotation = true;
            Rigidbody.useGravity = false;
        }

        /// <summary>
        /// Recalculate collider height, width and thickness.
        /// </summary>
        private void RecalculateColliderDimensions()
        {
            Collider.height = Height;
            Collider.center = Offset * Height;
            Collider.radius = Thickness / 2f;

            Collider.center += new Vector3(0f, StepHeightRatio * Collider.height / 2f, 0f);
            Collider.height *= 1f - StepHeightRatio;

            if (Collider.height / 2f < Collider.radius)
            {
                Collider.radius = Collider.height / 2f;
            }

            // Recalibrate sensor variables to fit new collider dimensions.
            if (_sensor != null)
            {
                RecalibrateSensor();
            }
        }

        /// <summary>
        /// Recalibrate sensor variables.
        /// </summary>
        private void RecalibrateSensor()
        {
            //Set sensor ray origin and direction.
            _sensor.Origin = ColliderCenter;
            _sensor.DirectionType = DirectionType.Down;

            // Calculate sensor layer mask.
            RecalculateSensorLayerMask();

            // Set sensor cast type;
            _sensor.SensorMode = Mode;

            // Calculate sensor radius/width.
            var radius = Thickness / 2f * RadiusModifier;

            // Multiply all sensor lengths with distance factor to compensate for floating point errors.
            const float factor = 0.001f;

            // Fit collider height to sensor radius.
            if (Collider)
            {
                radius = Mathf.Clamp(radius, factor, Collider.height / 2f * (1f - factor));
            }

            // Set sensor radius.
            _sensor.Radius = radius * transform.localScale.x;

            // Calculate and set sensor length.
            var length = 0f;
            length += Height * (1f - StepHeightRatio) * 0.5f;
            length += Height * StepHeightRatio;
            _extendedRange = length * (1f + factor) * transform.localScale.x;
            _sensor.MaximumDistance = length * transform.localScale.x;

            // Set sensor array variables.
            _sensor.Option.Rows = Rows;
            _sensor.Option.Count = Count;
            _sensor.Option.IsOffset = IsOffset;

#if UNITY_EDITOR

            _sensor.IsDebugMode = IsDebugMode;

#endif

            // Set sensor spherecast variables.
            _sensor.UseRealisticDistance = true;
            _sensor.UseRealisticSurfaceNormal = true;

            // Recalibrate sensor to the new values.
            _sensor.RecalibrateRaycastOriginPositions();
        }

        /// <summary>
        /// Recalculate sensor layer mask based on current physics settings.
        /// </summary>
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

        /// <summary>
        /// Check if mover is grounded.
        /// Store all relevant collision information for later.
        /// Calculate necessary adjustment velocity to keep the correct distance to the ground.
        /// </summary>
        public void Check()
        {
            // Check if object layer has been changed since last frame.
            // If so, recalculate sensor layer mask.
            if (_layer != gameObject.layer)
            {
                RecalculateSensorLayerMask();
            }

            // Reset ground adjustment velocity.
            _adjustmentVelocity = Vector3.zero;

            // Set sensor length.
            if (_useExtendedRange)
            {
                _sensor.MaximumDistance = _extendedRange + (Height * transform.localScale.x * StepHeightRatio);
            }
            else
            {
                _sensor.MaximumDistance = _extendedRange;
            }

            _sensor.Cast();

            // If sensor has not detected anything, set flags and return.
            if (_sensor.IsDetected == false)
            {
                IsGrounded = false;

                return;
            }

            // Set flags for ground detection.
            IsGrounded = true;

            // Get distance that sensor ray reached.
            var distance = _sensor.Distance;

            // Calculate how much mover needs to be moved up or down.
            var length = Height * transform.localScale.x * (1f - StepHeightRatio) * 0.5f;
            var middle = length + (Height * transform.localScale.x * StepHeightRatio);
            var difference = middle - distance;

            // Set new ground adjustment velocity for the next frame.
            _adjustmentVelocity = transform.up * (difference / Time.fixedDeltaTime);
        }

        /// <returns>
        /// True if mover is touching ground and the angle between hte 'up' vector and ground normal is not too steep.
        /// </returns>
        public bool IsGrounded { get; private set; }

        /// <summary>
        /// Set movement controller's velocity.
        /// </summary>
        public Vector3 Velocity { get => Rigidbody.velocity; set => Rigidbody.velocity = value + _adjustmentVelocity; }

        public bool UseExtendedRange { set => _useExtendedRange = value; }

        public float ColliderHeight
        {
            get => Height;
            set
            {
                Height = value;

                RecalculateColliderDimensions();
            }
        }

        /// <returns>
        /// The collider's center in world coordinates system.
        /// </returns>
        private Vector3 ColliderCenter => Collider.bounds.center;

        public float ColliderThickness
        {
            get => Thickness;
            set
            {
                if (value < 0f)
                {
                    value = 0f;
                }

                Thickness = value;

                RecalculateColliderDimensions();
            }
        }

        public float StopHeightRatio
        {
            set
            {
                value = Mathf.Clamp(value, 0f, 1f);

                StepHeightRatio = value;

                RecalculateColliderDimensions();
            }
        }

        public Vector3 GetGroundNormal()
        {
            return _sensor.Normal;
        }

        public Vector3 GetGroundPoint()
        {
            return _sensor.Position;
        }
    }
}
