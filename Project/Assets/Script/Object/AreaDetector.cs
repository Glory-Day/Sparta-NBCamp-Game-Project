using Backend.Util.Extension;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace Backend.Object
{
    [RequireComponent(typeof(BoxCollider))]
    public class AreaDetector : MonoBehaviour
    {
        [Header("Detection Settings")]
        [SerializeField] private LayerMask target;

        private SceneLoader _loader;

        private BoxCollider _boxCollider;

        private void Awake()
        {
            _loader = GetComponent<SceneLoader>();
            _boxCollider = GetComponent<BoxCollider>();
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            _boxCollider = GetComponent<BoxCollider>();
        }

        private void OnDrawGizmos()
        {
            var position = transform.TransformPoint(_boxCollider.center);
            var rotation = _boxCollider.transform.rotation;
            var size = _boxCollider.size;

            Handles.color = Color.yellow;
            using (new Handles.DrawingScope(Matrix4x4.TRS(position, rotation, Vector3.one)))
            {
                Handles.DrawWireCube(Vector3.zero, size);
            }
        }

#endif

        private void OnTriggerEnter(Collider other)
        {
            if (target.Has(other.gameObject.layer))
            {
                _loader.Load();
            }
        }
    }
}
