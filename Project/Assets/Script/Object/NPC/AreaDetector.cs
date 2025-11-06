using System;
using Backend.Util.Debug;
using Backend.Util.Extension;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace Backend.Object.NPC
{
    public class AreaDetector : MonoBehaviour
    {
        [Header("Detection Settings")]
        [SerializeField] private Collider detector;
        [SerializeField] private LayerMask target;

        [Header("Event Callbacks")]
        [Space(4f)]
        public UnityEvent onAreaEntered;

        public UnityEvent onAreaExited;

        private BoxCollider _boxCollider;
        private SphereCollider _sphereCollider;

#if UNITY_EDITOR

        private Color _color = Color.yellow;

#endif

        private void Awake()
        {
            if (detector == null)
            {
                Debugger.LogError("There is no detector.");
            }

            switch (detector)
            {
                case BoxCollider boxCollider:
                    _boxCollider = boxCollider;
                    break;
                case SphereCollider sphereCollider:
                    _sphereCollider = sphereCollider;
                    break;
            }
        }

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            Handles.color = _color;

            switch (detector)
            {
                case BoxCollider boxCollider:
                {
                    var position = transform.TransformPoint(boxCollider.center);
                    var rotation = boxCollider.transform.rotation;
                    var size = boxCollider.size;

                    using (new Handles.DrawingScope(Matrix4x4.TRS(position, rotation, Vector3.one)))
                    {
                        Handles.DrawWireCube(Vector3.zero, size);
                    }

                    break;
                }
                case SphereCollider sphereCollider:
                {
                    var position = transform.TransformPoint(sphereCollider.center);
                    var rotation = sphereCollider.transform.rotation;
                    var radius = sphereCollider.radius;

                    using (new Handles.DrawingScope(Matrix4x4.TRS(position, rotation, Vector3.one)))
                    {
                        Handles.DrawWireDisc(Vector3.zero, Vector3.up, radius);
                        Handles.DrawWireDisc(Vector3.zero, Vector3.right, radius);
                        Handles.DrawWireDisc(Vector3.zero, Vector3.forward, radius);
                    }

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

#endif

        private void OnTriggerEnter(Collider other)
        {
            if (target.Has(other.gameObject.layer))
            {
#if UNITY_EDITOR

                _color = Color.red;

#endif

                onAreaEntered.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (target.Has(other.gameObject.layer))
            {
#if UNITY_EDITOR

                _color = Color.yellow;

#endif

                onAreaExited.Invoke();
            }
        }

        #region NESTED ENUMERATOR API

        private enum DetectionMode
        {
            BoxCollider,
            SphereCollider
        }

        #endregion
    }
}
