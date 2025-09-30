using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Backend.Object.Character.Player
{
    public class Sensor
    {
        private readonly Transform _transform;

        private readonly RaycastExcluder _excluder;

        // Starting point of ray cast.
        private Vector3 _origin = Vector3.zero;
        private DirectionType _directionType;

        // Backup normal used for specific edge cases when using sphere casts.
        private Vector3 _cache;

        private Vector3[] _origins;

        public Sensor(Transform transform, Collider collider)
        {
            _transform = transform;

            if (collider == null)
            {
                return;
            }

            _excluder = new RaycastExcluder(new [] { collider });
        }

        /// <summary>
        /// Cast a ray (or sphere or array of rays) to check for colliders.
        /// </summary>
        public void Cast()
        {
            Hits.Clear();

            // Calculate origin and direction of ray in world coordinates system.
            var origin = _transform.TransformPoint(Origin);
            var direction = Direction;

            _excluder.Apply();

            // Depending on the chosen mode of detection, call different functions to check for colliders.
            switch (CastMode)
            {
                case CastMode.SingleRay:
                    CastBySingleRay(origin, direction);
                    break;
                case CastMode.MultipleRay:
                    CastByMultipleRay(origin, direction);
                    break;
                default:
                    IsDetected = false;
                    break;
            }

            _excluder.Restore();
        }

        /// <summary>
        /// Cast a single ray into direction from origin position.
        /// </summary>
        private void CastBySingleRay(Vector3 origin, Vector3 direction)
        {
            IsDetected = Physics.Raycast(origin, direction, out var hit, MaximumDistance, LayerMask, QueryTriggerInteraction.Ignore);

            if (IsDetected == false)
            {
                return;
            }

            Position = hit.point;
            Normal = hit.normal;
            Distance = hit.distance;
        }

        /// <summary>
        /// Cast an array of rays into direction and centered around origin.
        /// </summary>
        private void CastByMultipleRay(Vector3 origin, Vector3 direction)
        {
            // Calculate origin and direction of ray in world coordinates system.
            var position = Vector3.zero;
            var rayDirection = Direction;

            for (var i = 0; i < _origins.Length; i++)
            {
                // Calculate ray origin position.
                position = origin + _transform.TransformDirection(_origins[i]);

                var isHit = Physics.Raycast(position, rayDirection, out var hit, MaximumDistance, LayerMask, QueryTriggerInteraction.Ignore);
                if (isHit == false)
                {
                    continue;
                }

                Hits.Add(new HitInformation(hit));
            }

            IsDetected = Hits.Count > 0;

            if (IsDetected == false)
            {
                return;
            }

            // Calculate average surface normal.
            var normal = Hits.Aggregate(Vector3.zero, (current, hit) => current + hit.Normal);
            normal.Normalize();

            //Calculate average surface point;
            var point = Hits.Aggregate(Vector3.zero, (current, hit) => current + hit.Position);
            point /= Hits.Count;

            Position = point;
            Normal = normal;
            Distance = (origin - Position).Project(direction).magnitude;
        }

        /// <summary>
        /// Recalculate start positions for the raycast array
        /// </summary>
        public void RecalibrateRaycastOriginPositions()
        {
            _origins = GetRaycastOriginPositions(Option.Rows, Radius, Option.Count, Option.IsOffset);
        }

        /// <returns>
        /// An array containing the starting positions of all array rays (in local coordinates system) based on the input arguments.
        /// </returns>
        public static Vector3[] GetRaycastOriginPositions(int rows, float radius, int count, bool isOffset)
        {
            // Initialize list used to store the positions.
            // Add central start position to the list.
            var positions = new List<Vector3> { Vector3.zero };

            for (var i = 0; i < rows; i++)
            {
                // Calculate radius for all positions on this row.
                var r = (float)(i + 1) / rows;

                for (var j = 0; j < count * (i + 1); j++)
                {
                    // Calculate angle (in degrees) for this individual position.
                    var angle = 360f / (count * (i + 1)) * j;

                    //If 'offsetRows' is set to 'true', every other row is offset;
                    if (isOffset && i % 2 == 0)
                    {
                        angle += 360f / (count * (i + 1)) / 2f;
                    }

                    // Combine radius and angle into one position and add it to the list.
                    var x = r * Mathf.Cos(Mathf.Deg2Rad * angle);
                    var y = r * Mathf.Sin(Mathf.Deg2Rad * angle);

                    positions.Add(new Vector3(x, 0f, y) * radius);
                }
            }

            // Convert list to array and return array.
            return positions.ToArray();
        }

        /// <summary>
        /// Set the position for the raycast to start from.
        /// </summary>
        public Vector3 Origin
        {
            get => _origin;
            set
            {
                if (_transform == null)
                {
                    return;
                }

                _origin = _transform.InverseTransformPoint(value);
            }
        }

        /// <summary>
        /// Set axis of this instance's transform will be used as the direction for the raycast.
        /// </summary>
        public DirectionType DirectionType
        {
            set
            {
                if (_transform == null)
                {
                    return;
                }

                _directionType = value;
            }
        }

        /// <returns>
        /// Calculate a direction in world coordinates based on the local axes of this instance's transform component.
        /// </returns>
        private Vector3 Direction
        {
            get => _directionType switch
            {
                DirectionType.Up => _transform.up,
                DirectionType.Down => -_transform.up,
                DirectionType.Left => -_transform.right,
                DirectionType.Right => _transform.right,
                DirectionType.Forward => _transform.forward,
                DirectionType.Backward => -_transform.forward,
                _ => Vector3.one
            };
        }

        public CastMode CastMode { get; set; } = CastMode.SingleRay;
        public LayerMask LayerMask { get; set; } = 255;

        public float MaximumDistance { get; set; } = 1f;
        public float Radius { get; set; } = 0.2f;

        public bool IsDetected { get; private set; }

        public Vector3 Position { get; private set; }
        public Vector3 Normal { get; private set; }
        public float Distance { get; private set; }

        /// <summary>
        /// Cast an additional ray to get the true surface normal.
        /// </summary>
        public bool UseRealisticSurfaceNormal { get; set; }

        /// <summary>
        /// Cast an additional ray to get the true distance to the ground.
        /// </summary>
        public bool UseRealisticDistance { get; set; }

#if UNITY_EDITOR

        // Whether to draw debug information in the editor.
        public bool IsDebugMode { get; set; }

#endif

        public List<HitInformation> Hits { get; } = new();

        public MultipleRayOption Option { get; } = new();

        #region NESTED STRUCTURE API

        public struct HitInformation
        {
            public HitInformation(RaycastHit hit)
            {
                Transform = hit.transform;
                Collider = hit.collider;

                Position = hit.point;
                Normal = hit.normal;
                Distance = hit.distance;
            }

            public Transform Transform { get; set; }
            public Collider Collider { get; set; }

            public Vector3 Position { get; set; }
            public Vector3 Normal { get; set; }
            public float Distance { get; set; }
        }

        public class MultipleRayOption
        {
            // Number of rays in every row.
            public int Rows { get; set; } = 3;

            // Number of rows around the central ray;
            public int Count { get; set; } = 9;

            // Whether offset every other row.
            public bool IsOffset { get; set; }
        }

        #endregion
    }
}
