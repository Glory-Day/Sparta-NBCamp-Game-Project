#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Text;
using Script.Util.Extension;
using UnityEditor;
using UnityEngine;

namespace Backend.Object.Character.Player
{
    public partial class AdvancedActionController
    {
        private Vector3 _velocity = Vector3.zero;

        private Color _color = Color.blue;

        private void OnDrawGizmos()
        {
            DrawLabels();
            DrawMomentumVector();
            DrawWireCapsuleCollider();
        }

        private void DrawLabels()
        {
            var label = State switch
            {
                State.Grounded => "Grounded",
                State.Sliding => "Sliding",
                State.Falling => "Falling",
                State.Rising => "Rising",
                State.Rolling => "Rolling",
                State.Attacking => "Attacking",
                _ => throw new ArgumentOutOfRangeException()
            };

            var builder = new StringBuilder();
            builder.AppendLine(label);
            builder.AppendLine($"Bufferable: {IsButtonBufferable}");

            Handles.color = Color.white;
            Handles.Label(transform.position, builder.ToString());
        }

        private void DrawMomentumVector()
        {
            var child = transform.GetChild(0);

            var a = Composer.PerspectiveController.Forward.normalized;
            var b = _velocity.normalized;
            var normal = child.right;

            a = Vector3.ProjectOnPlane(a, normal).normalized;
            b = Vector3.ProjectOnPlane(b, normal).normalized;

            const float size = 0.08f;
            const float radius = 0.4f;
            var angle = Vector3.SignedAngle(a, b, normal);
            var position = transform.position;
            var rotation = Quaternion.identity;

            Handles.color = new Color(0f, 0f, 1f, 0.2f);
            Handles.DrawSolidArc(transform.position, normal, a, angle, radius);

            Handles.color = Color.magenta;
            Handles.DrawLine(position, position + (a * radius));
            Handles.DrawLine(position, position + (b * radius));

            // Draw facing direction line for debugging.
            Gizmos.color = Color.magenta;
            Handles.SphereHandleCap(0, position + (a * radius), rotation, size, EventType.Repaint);
            Handles.SphereHandleCap(0, position + (b * radius), rotation, size, EventType.Repaint);
        }

        private void DrawWireCapsuleCollider()
        {
            var local = Detector.transform;
            var center = local.TransformPoint(Detector.center);
            var radius = Detector.radius;
            var height = Detector.height;
            var axis = Detector.GetDirection(transform);
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

            const int segments = 18;
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
    }
}

#endif
