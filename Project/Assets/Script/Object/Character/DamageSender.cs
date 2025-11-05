using Script.Util.Extension;
using UnityEditor;
using UnityEngine;

namespace Backend.Object.Character
{
    public class DamageSender : MonoBehaviour
    {
        [Header("Data Information")]
        [SerializeField] private float physicalDamagePoint;
        [SerializeField] private float magicalDamagePoint;

        [Header("Detection References")]
        [SerializeField] private CapsuleCollider detector;

        [Header("Detection Settings")]
        [SerializeField] private LayerMask layerMask = ~0;

        private CapsuleCollider _collider;

#if UNITY_EDITOR

        private Color _color = Color.blue;

#endif

        private void Awake()
        {
            _collider = GetComponent<CapsuleCollider>();
            _collider.enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((layerMask.value & (1 << other.gameObject.layer)) == 0)
            {
                return;
            }

            var status = other.GetComponent<Status>();
            status?.TakeDamage(PhysicalDamagePoint, null);
        }

        public void StartDetection()
        {
            _collider.enabled = true;

#if UNITY_EDITOR

            _color = Color.red;

#endif
        }

        public void StopDetection()
        {
            _collider.enabled = false;

#if UNITY_EDITOR

            _color = Color.blue;

#endif
        }

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            DrawWireCapsuleCollider();
        }

        private void DrawWireCapsuleCollider()
        {
            var local = detector.transform;
            var center = local.TransformPoint(detector.center);
            var radius = detector.radius;
            var height = detector.height;
            var axis = detector.GetDirection(transform);
            var offset = Mathf.Max(0f, (height * 0.5f) - radius);

            var top = center + (axis * offset);
            var bottom = center - (axis * offset);

            Handles.color = _color;
            if (offset <= Mathf.Epsilon)
            {
                Handles.DrawWireDisc(center, transform.up, radius);
                Handles.DrawWireDisc(center, transform.right, radius);
                Handles.DrawWireDisc(center, transform.forward, radius);
            }

            var normal = axis.normalized;
            var tangent = Vector3.Cross(normal, Vector3.up);
            if (tangent.sqrMagnitude < 1e-6f)
            {
                tangent = Vector3.Cross(normal, Vector3.forward);
            }

            tangent.Normalize();
            var bitangent = Vector3.Cross(normal, tangent);

            const int segments = 36;
            var a = new Vector3[segments];
            var b = new Vector3[segments];
            for (int i = 0; i < segments; i++)
            {
                var angle = 2f * Mathf.PI * i / segments;
                var delta = (tangent * Mathf.Cos(angle) * radius) + (bitangent * Mathf.Sin(angle) * radius);

                a[i] = top + delta;
                b[i] = bottom + delta;
            }

            Handles.color = _color;
            for (int i = 0; i < segments; i++)
            {
                var j = (i + 1) % segments;
                Handles.DrawLine(a[i], a[j]);
                Handles.DrawLine(b[i], b[j]);
                Handles.DrawLine(a[i], b[i]);
            }

            Handles.DrawWireArc(top, tangent, -normal, 180f, radius);
            Handles.DrawWireArc(top, bitangent, -normal, 180f, radius);
            Handles.DrawWireArc(top, tangent, normal, 180f, radius);
            Handles.DrawWireArc(top, bitangent, normal, 180f, radius);
            Handles.DrawWireDisc(top, normal, radius);

            Handles.DrawWireArc(bottom, tangent, -normal, 180f, radius);
            Handles.DrawWireArc(bottom, bitangent, -normal, 180f, radius);
            Handles.DrawWireArc(bottom, tangent, normal, 180f, radius);
            Handles.DrawWireArc(bottom, bitangent, normal, 180f, radius);
            Handles.DrawWireDisc(bottom, -normal, radius);
        }

#endif

        public float PhysicalDamagePoint
        {
            get => physicalDamagePoint;
            set => physicalDamagePoint = value;
        }

        public float MagicalDamagePoint
        {
            get => magicalDamagePoint;
            set => magicalDamagePoint = value;
        }
    }
}
