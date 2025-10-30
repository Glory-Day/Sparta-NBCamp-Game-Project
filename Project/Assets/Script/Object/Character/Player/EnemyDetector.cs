using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace Backend.Object.Character.Player
{
    public class EnemyDetector : MonoBehaviour
    {
        [Header("Detection Settings")]
        [SerializeField] private float radius;
        [SerializeField] private LayerMask layer;
        [SerializeField] private int maximumCount;

        private Collider[] _hits;

        public void Awake()
        {
            _hits = new Collider[maximumCount];
        }

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            const float size = 0.2f;

            Handles.color = Color.yellow;
            Handles.DrawWireDisc(transform.position, Vector3.up, radius);

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

                Gizmos.color = _hits[i].transform == NearestEnemy ? Color.red : Color.blue;

                Gizmos.DrawWireSphere(transform.position, size);
                Gizmos.DrawWireSphere(_hits[i].transform.position, size);
                Gizmos.DrawLine(transform.position, _hits[i].transform.position);
            }
        }

#endif

        public void FidNearestEnemy()
        {
            var position = transform.position;

            var length = Physics.OverlapSphereNonAlloc(position, radius, _hits, layer);
            if (length == 0)
            {
                NearestEnemy = null;

                return;
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

                NearestEnemy = _hits[i].transform;
            }
        }

        public Transform NearestEnemy { get; private set; }
    }
}
