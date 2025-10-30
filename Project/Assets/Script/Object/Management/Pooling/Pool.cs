using System.Collections.Generic;
using UnityEngine;

namespace Backend.Object.Management.Pooling
{
    public class Pool
    {
        private readonly List<Clone> _clones;
        private readonly Dictionary<GameObject, Clone> _activeClones;

        private readonly GameObject _componentType;
        private readonly Transform _parentTransform;

        private int _index;

        public int TotalCloneCount => _clones.Count;

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
            Clone clone = new (cloneInstance);

            cloneInstance.SetActive(false);

            _clones.Add(clone);
            return clone;
        }

        public void Release(GameObject key)
        {
            if (_activeClones.TryGetValue(key, out Clone clone) == false)
            {
                return;
            }

            clone.Release();
            _activeClones.Remove(key);
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

                clone ??= CreateClone();

                clone.Use();
                _activeClones.Add(clone.Instance, clone);

                return clone.Instance;
            }
        }
    }
}
