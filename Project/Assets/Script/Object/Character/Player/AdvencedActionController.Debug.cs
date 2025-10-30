#if UNITY_EDITOR

using Script.Util.Extension;
using UnityEditor;
using UnityEngine;

namespace Backend.Object.Character.Player
{
    public partial class AdvancedActionController
    {
        private Vector3 _velocity = Vector3.zero;

        private void OnDrawGizmos()
        {
            if (_movementController == null)
            {
                return;
            }

            var child = transform.GetChild(0);

            var a = _facing.normalized;
            var b = _velocity.normalized;
            var normal = child.right;

            a = Vector3.ProjectOnPlane(a, normal).normalized;
            b = Vector3.ProjectOnPlane(b, normal).normalized;

            var angle = Vector3.SignedAngle(a, b, normal);
            const float radius = 0.4f;

            Handles.color = new Color(0f, 0f, 1f, 0.2f);
            Handles.DrawSolidArc(transform.position, normal, a, angle, radius);

            Debug.DrawLine(transform.position, transform.position + (a * radius), Color.blue);
            Debug.DrawLine(transform.position, transform.position + (b * radius), Color.blue);

            // Draw facing direction line for debugging.
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position + (a * radius), 0.04f);
            Gizmos.DrawSphere(transform.position + (b * radius), 0.04f);

            DrawCapsuleCollider();
        }

        private void DrawCapsuleCollider()
        {
            var center = detectionCollider.center;
            var axis = detectionCollider.GetDirection(transform);
            var radius = detectionCollider.radius;
            var point = transform.TransformPoint(center);
            var delta = Mathf.Max(0f, (detectionCollider.height * 0.5f) - radius);

            var a = point + (axis * delta);
            var b = point - (axis * delta);

            Gizmos.color = _color;
            if (delta <= Mathf.Epsilon)
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
    }
}

#endif
