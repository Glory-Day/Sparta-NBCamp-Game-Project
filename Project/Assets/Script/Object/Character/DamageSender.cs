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

        [Header("Overlap Settings")]
        [SerializeField] private Vector3 center = Vector3.zero;
        [SerializeField] private float radius = 0.5f;
        [SerializeField] private float height = 1.8f;
        [SerializeField] private Vector3 up = Vector3.up;

        [Header("Debug Settings")]
        [SerializeField] private Color color = new (0f, 0.6f, 1f, 1f);
        [SerializeField] private int segment = 20;

        private CapsuleCollider _collider;

        private void Awake()
        {
            _collider = GetComponent<CapsuleCollider>();
            _collider.enabled = false;
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            if (radius < 0f)
            {
                radius = 0f;
            }

            if (height < 0f)
            {
                height = 0f;
            }
        }

#endif

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

            color = Color.red;

#endif
        }

        public void StopDetection()
        {
            _collider.enabled = false;

#if UNITY_EDITOR

            color = new Color(0f, 0.6f, 1f, 1f);

#endif
        }

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            var point = transform.TransformPoint(center);
            var axis = up == Vector3.zero ? transform.up : transform.TransformDirection(up.normalized);
            var offset = Mathf.Max(0f, (height * 0.5f) - radius);

            var a = point + (axis * offset);
            var b = point - (axis * offset);

            Gizmos.color = color;
            if (offset <= Mathf.Epsilon)
            {
                Gizmos.DrawWireSphere(point, radius);
            }
            else
            {
                DrawWireCapsule(a, b, radius, axis, segment);
            }
        }

#endif

        private void DrawWireCapsule(Vector3 a, Vector3 b, float r, Vector3 axis, int segments)
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
                var offset = (tangent * Mathf.Cos(angle) * r) + (bitangent * Mathf.Sin(angle) * r);
                c1[i] = a + offset;
                c2[i] = b + offset;
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
