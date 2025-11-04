using System;
using Backend.Object.Character.Enemy;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace Backend.Object.Character.Player
{
    public class EnemyCharacterFinder : MonoBehaviour
    {
        #region SERIALIZABLE PROPERTIES API

        [field: Header("Detection Settings")]
        [field: SerializeField] public float Radius { get; private set; }
        [field: SerializeField] public int MaximumCount { get; private set; }

        [field: Header("Layer Overrides")]
        [field: SerializeField] public LayerMask IncludedLayers { get; private set; }

        #endregion

#if UNITY_EDITOR

        private Transform _target;

#endif

        private Collider[] _hits;

        public void Awake()
        {
            _hits = new Collider[MaximumCount];
        }

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            const float size = 0.2f;

            Handles.color = Color.yellow;
            Handles.DrawWireDisc(transform.position, Vector3.up, Radius);

            if (_hits == null || _hits.Length == 0)
            {
                return;
            }

            var length = _hits.Length;
            for (var i = 0; i < length; i++)
            {
                if (_hits[i] == null)
                {
                    continue;
                }

                Handles.color = _hits[i].transform == _target ? Color.red : Color.blue;
                Handles.SphereHandleCap(0, transform.position, Quaternion.identity, size * 2f, EventType.Repaint);
                Handles.SphereHandleCap(0, _hits[i].transform.position, Quaternion.identity, size * 2f, EventType.Repaint);
                Handles.DrawLine(transform.position, _hits[i].transform.position);
            }
        }

#endif

        public EnemyStatus FindNearestEnemyStatus()
        {
            EnemyStatus target = null;

            var position = transform.position;
            var length = Physics.OverlapSphereNonAlloc(position, Radius, _hits, IncludedLayers);
            if (length == 0)
            {
                return null;
            }

            var cache = float.MaxValue;
            for (var i = 0; i < length; i++)
            {
                var a = _hits[i].transform.position;
                var b = transform.position;
                var distance = Vector3.SqrMagnitude(a - b);

                if (distance >= cache)
                {
                    continue;
                }

                cache = distance;
                target = _hits[i].GetComponent<EnemyStatus>();

#if UNITY_EDITOR

                _target = target.transform;

#endif
            }

            return target;
        }

        public void Clear()
        {
            Array.Clear(_hits, 0, MaximumCount);
        }
    }
}
