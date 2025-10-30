using System.Collections.Generic;
using System.Linq;
using Backend.Object.Management.Pooling;
using Backend.Util.Management;
using UnityEngine;
using Debugger = Backend.Util.Debug.Debugger;

namespace Backend.Object.Management
{
    public class ObjectPoolManager : SingletonGameObject<ObjectPoolManager>
    {
        private readonly Dictionary<GameObject, Pool> _poolObjects = new();
        private readonly Dictionary<GameObject, Pool> _poolClones = new();

        private void CreatePoolObject_Internal(GameObject origin, int capacity, Transform parent = null)
        {
            if (_poolObjects.TryGetValue(origin, out Pool exitPool))
            {
                exitPool.Resize(capacity);
                Debugger.LogError($"Pool Object {origin} is already created, Now Change Capacity => {capacity}");
                return;
            }

            Pool pool = new Pool(origin, capacity, parent);
            _poolObjects[origin] = pool;
        }

        private GameObject SpawnPoolObject_Internal(GameObject origin, Vector3? pos, Quaternion? rot, Transform parent)
        {
            if (!_poolObjects.ContainsKey(origin))
            {
                CreatePoolObject(origin, 1, parent != null ? parent : Instance.transform);
            }

            Pool instance = _poolObjects[origin];
            GameObject clone = instance.Object;

            clone.transform.SetPositionAndRotation(pos ?? Vector3.zero, rot ?? Quaternion.identity);
            _poolClones[clone] = instance;

            clone.SetActive(true);

            return clone;
        }

        private void Release_Internal(GameObject clone)
        {
            clone.SetActive(false);

            if (_poolClones.ContainsKey(clone))
            {
                // Release and remove deactivated object.
                _poolClones[clone].Release(clone);
                _poolClones.Remove(clone);
            }
        }

        private void ReleaseAll_Internal()
        {
            var count = transform.childCount;
            for (var i = 0; i < count; i++)
            {
                Release_Internal(transform.GetChild(i).gameObject);
            }
        }


        private void DestroyPoolObject_Internal(GameObject origin)
        {
            if (_poolObjects.TryGetValue(origin, out Pool pool))
            {
                pool.DestroyClone();
                _poolClones.Remove(origin);
                _poolObjects.Remove(origin);
            }
        }

        private void DestroyPoolObject_Internal()
        {
            foreach(Pool pool in _poolObjects.Values)
            {
                pool.DestroyClone();
            }
            _poolClones.Clear();
            _poolObjects.Clear();
        }

        private int GetPoolObjectCount_Internal(GameObject origin)
        {
            if (_poolObjects.TryGetValue(origin, out Pool pool))
            {
                return pool.TotalCloneCount;
            }
            return 0;
        }

        public static void CreatePoolObject(GameObject origin, int capacity, Transform parent = null)
        {
            Instance.CreatePoolObject_Internal(origin, capacity, parent);
        }

        public static GameObject SpawnPoolObject(GameObject origin, Vector3? pos, Quaternion? rot, Transform parent)
        {
            return Instance.SpawnPoolObject_Internal(origin, pos, rot, parent);
        }

        public static void Release(GameObject clone)
        {
            Instance.Release_Internal(clone);
        }

        public static void ReleaseAll()
        {
            Instance.ReleaseAll_Internal();
        }

        public static void DestroyPoolObject(GameObject origin)
        {
            Instance.DestroyPoolObject_Internal(origin);
        }

        public static void DestroyPoolObject()
        {
            Instance.DestroyPoolObject_Internal();
        }

        public static int GetPoolObjectCount(GameObject origin)
        {
            return Instance.GetPoolObjectCount_Internal(origin);
        }
    }
}
