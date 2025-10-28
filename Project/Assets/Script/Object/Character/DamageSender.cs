using Script.Util.Extension;
using UnityEngine;

namespace Backend.Object.Character
{
    public class DamageSender : MonoBehaviour
    {
        [Header("Data Information")]
        [SerializeField] private float physicalDamagePoint;
        [SerializeField] private float magicalDamagePoint;

        [Header("Detection Settings")]
        [SerializeField] private LayerMask layerMask = ~0;

        private CapsuleCollider _collider;

#if UNITY_EDITOR

        private Color _color = new (0f, 0.6f, 1f, 1f);

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
            status.TakeDamage(PhysicalDamagePoint, null);
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

            _color = new Color(0f, 0.6f, 1f, 1f);

#endif
        }

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            if (_collider == null)
            {
                return;
            }

            var center = _collider.center;
            var axis = _collider.GetDirection(transform);
            var height = _collider.height;
            var radius = _collider.radius;
            var point = transform.TransformPoint(center);
            var offset = Mathf.Max(0f, (height * 0.5f) - radius);

            var a = point + (axis * offset);
            var b = point - (axis * offset);

            Gizmos.color = _color;
            if (offset <= Mathf.Epsilon)
            {
                Gizmos.DrawWireSphere(point, radius);
            }
            else
            {
                DrawWireCapsule(a, b, radius, axis);
            }
        }

        private void DrawWireCapsule(Vector3 a, Vector3 b, float r, Vector3 axis, int segments = 20)
        {
            var normal = axis.normalized;
            var tangent = Vector3.Cross(normal, Vector3.up);
            if (tangent.sqrMagnitude < 1e-6f)
            {
                tangent = Vector3.Cross(normal, Vector3.forward);
            }

            tangent.Normalize();
            var bitangent = Vector3.Cross(normal, tangent);

            var c1 = new Vector3[segments];
            var c2 = new Vector3[segments];
            for (int i = 0; i < segments; i++)
            {
                var angle = 2f * Mathf.PI * i / segments;
                var delta = (tangent * Mathf.Cos(angle) * r) + (bitangent * Mathf.Sin(angle) * r);
                c1[i] = a + delta;
                c2[i] = b + delta;
            }

            for (int i = 0; i < segments; i++)
            {
                var ni = (i + 1) % segments;
                Gizmos.DrawLine(c1[i], c1[ni]);
                Gizmos.DrawLine(c2[i], c2[ni]);
                Gizmos.DrawLine(c1[i], c2[i]);
            }

            Gizmos.DrawWireSphere(a, r);
            Gizmos.DrawWireSphere(b, r);
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
