using System.Collections.Generic;
using Backend.Util.Management;
using UnityEngine;
using Debugger = Backend.Util.Debug.Debugger;

namespace Backend.Object.Management
{
    [System.Serializable]
    public class Clone
    {
        public Clone(GameObject instance)
        {
            Instance = instance;
        }

        public GameObject Instance;

        public void Use()
        {
            IsUse = true;
        }
        public void Release()
        {
            IsUse = false;
        }
        public bool IsUse { get; private set; }
    }
    public class Pool
    {
        private readonly List<Clone> _clones;
        private readonly Dictionary<GameObject, Clone> _activeClones;

        private readonly GameObject _componentType;
        private readonly Transform _parentTransform;

        private int _index;

        public int TotalCloneCount
        {
            get { return _clones.Count; }
            private set { }
        }

        public Pool(GameObject origin, int capacity, Transform parent = null)
        {
            _clones = new List<Clone>(capacity);
            _activeClones = new Dictionary<GameObject, Clone>(capacity);
            _componentType = origin;
            _parentTransform = parent;

            Init(capacity);
        }

        private void Init(int capacity)
        {
            for (int i = 0; i < capacity; i++)
            {
                CreateClone();
            }
        }

        private Clone CreateClone()
        {
            if (_componentType == null)
            {
                return null;
            }

            //Create new Clone.
            GameObject cloneInstance = UnityEngine.Object.Instantiate(_componentType, _parentTransform);
            Clone clone = new Clone(cloneInstance);

            cloneInstance.SetActive(false);

            _clones.Add(clone);
            return clone;
        }

        public void Release(GameObject key)
        {
            if (_activeClones.TryGetValue(key, out Clone clone))
            {
                clone.Release();
                _activeClones.Remove(key);
                return;
            }
        }

        public void DestroyClone()
        {
            foreach(Clone clone in _clones)
            {
                UnityEngine.Object.Destroy(clone.Instance);
            }
            _clones.Clear();
            _activeClones.Clear();
        }

        public void Resize(int capacity)
        {
            if (capacity <= _clones.Count)
            {
                return;
            }

            int addCount = capacity - _clones.Count;

            for(int i = 0; i < addCount; i++)
            {
                CreateClone();
            }
        }

        public GameObject Object
        {
            get
            {
                Clone clone = null;

                int count = _clones.Count;
                for (int i = 0; i < count; i++)
                {
                    _index++;
                    if(_index > count - 1)
                    {
                        _index = 0;
                    }

                    if (_clones[_index].IsUse)
                    {
                        continue;
                    }

                    clone = _clones[_index];
                    break;
                }

                if (clone == null)
                {
                    clone = CreateClone();
                }

                clone.Use();
                _activeClones.Add(clone.Instance, clone);

                return clone.Instance;
            }
        }
    }


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
                return;
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
