using System;
using System.Collections;
using System.Collections.Generic;
using Backend.Util.Debug;
using UnityEngine;

namespace Backend.Object.Character
{
    public class DamageSender : MonoBehaviour
    {
        [Header("Capsule (local space)")] [SerializeField]
        private Vector3 center = Vector3.zero;

        [SerializeField] private float radius = 0.5f;
        [SerializeField] private float height = 1.8f;
        [SerializeField] private Vector3 up = Vector3.up;

        [Header("Detection")] [SerializeField] private LayerMask layerMask = ~0;
        [SerializeField] private QueryTriggerInteraction queryTrigger = QueryTriggerInteraction.UseGlobal;

        [Header("Gizmos")] [SerializeField] private bool drawWhenSelectedOnly = true;
        [SerializeField] private Color shapeColor = new (0f, 0.6f, 1f, 1f);
        [SerializeField] private Color hitColor = new (1f, 0.2f, 0.2f, 1f);
        [SerializeField] private int circleSegments = 20;

        private Collider[] _lastHits = Array.Empty<Collider>();
        private Coroutine _detectCoroutine = null;
        private float _detectInterval = 0.1f;
        private bool _useRealtime = false;

        private HashSet<int> _hits = new HashSet<int>();

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

        // Runtime API
        public void StartDetection(float intervalSec = 0.1f, bool realtime = false)
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("StartDetection only works in Play mode.");

                return;
            }

            _detectInterval = Mathf.Max(0.01f, intervalSec);
            _useRealtime = realtime;
            if (_detectCoroutine != null)
            {
                StopCoroutine(_detectCoroutine);
            }

            _detectCoroutine = StartCoroutine(DetectionRoutine());
        }

        public void StopDetection()
        {
            if (_detectCoroutine == null)
            {
                return;
            }

            StopCoroutine(_detectCoroutine);
            _detectCoroutine = null;
            _hits.Clear();
        }

        public void RefreshOverlap()
        {
            Vector3 worldCenter = transform.TransformPoint(center);
            Vector3 axis = (up == Vector3.zero) ? transform.up : transform.TransformDirection(up.normalized);
            float halfOffset = Mathf.Max(0f, (height * 0.5f) - radius);
            Vector3 p1 = worldCenter + (axis * halfOffset);
            Vector3 p2 = worldCenter - (axis * halfOffset);

            _lastHits = Physics.OverlapCapsule(p1, p2, radius, layerMask, queryTrigger);

            for (var i = 0; i < _lastHits.Length; i++)
            {
                var id = _lastHits[i].gameObject.GetInstanceID();

                _hits.Add(id);
            }

            if (_lastHits.Length > 0)
            {
                Debugger.LogMessage($"Detected: {_lastHits.Length} is hit!");
            }
        }

        private IEnumerator DetectionRoutine()
        {
            while (true)
            {
                RefreshOverlap();
                if (_useRealtime)
                {
                    yield return new WaitForSecondsRealtime(_detectInterval);
                }
                else
                {
                    yield return new WaitForSeconds(_detectInterval);
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (drawWhenSelectedOnly && !UnityEditor.Selection.Contains(gameObject))
            {
                return;
            }

            DrawVisualization();
        }

        private void OnDrawGizmosSelected()
        {
            if (!drawWhenSelectedOnly)
            {
                DrawVisualization();
            }
        }

        private void DrawVisualization()
        {
            Vector3 worldCenter = transform.TransformPoint(center);
            Vector3 axis = (up == Vector3.zero) ? transform.up : transform.TransformDirection(up.normalized);
            float halfOffset = Mathf.Max(0f, (height * 0.5f) - radius);
            Vector3 p1 = worldCenter + axis * halfOffset;
            Vector3 p2 = worldCenter - axis * halfOffset;

            Gizmos.color = shapeColor;
            if (halfOffset <= Mathf.Epsilon)
            {
                Gizmos.DrawWireSphere(worldCenter, radius);
            }
            else
            {
                DrawWireCapsule(p1, p2, radius, axis, circleSegments);
            }

            if (_lastHits == null || _lastHits.Length <= 0)
            {
                return;
            }

            Gizmos.color = hitColor;
            foreach (var c in _lastHits)
            {
                if (c == null) continue;
                Vector3 closest = c.ClosestPoint(worldCenter);
                Gizmos.DrawSphere(closest, Mathf.Max(0.02f, radius * 0.08f));
                Gizmos.DrawLine(closest, worldCenter);
            }
        }

        // 간단한 wire-capsule 렌더러
        private void DrawWireCapsule(Vector3 p1, Vector3 p2, float r, Vector3 axis, int segments)
        {
            Vector3 a = axis.normalized;
            Vector3 tangent = Vector3.Cross(a, Vector3.up);
            if (tangent.sqrMagnitude < 1e-6f)
            {
                tangent = Vector3.Cross(a, Vector3.forward);
            }

            tangent.Normalize();
            Vector3 bitangent = Vector3.Cross(a, tangent);

            Vector3[] c1 = new Vector3[segments];
            Vector3[] c2 = new Vector3[segments];
            for (int i = 0; i < segments; i++)
            {
                float ang = (2f * Mathf.PI * i) / segments;
                Vector3 offset = tangent * Mathf.Cos(ang) * r + bitangent * Mathf.Sin(ang) * r;
                c1[i] = p1 + offset;
                c2[i] = p2 + offset;
            }

            for (int i = 0; i < segments; i++)
            {
                int ni = (i + 1) % segments;
                Gizmos.DrawLine(c1[i], c1[ni]);
                Gizmos.DrawLine(c2[i], c2[ni]);
                Gizmos.DrawLine(c1[i], c2[i]);
            }

            Gizmos.DrawWireSphere(p1, r);
            Gizmos.DrawWireSphere(p2, r);
        }
    }
}
