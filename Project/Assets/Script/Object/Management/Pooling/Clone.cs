using UnityEngine;

namespace Backend.Object.Management.Pooling
{
    [System.Serializable]
    public class Clone
    {
        public Clone(GameObject instance)
        {
            Instance = instance;
        }

        public GameObject Instance { get; private set; }

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
}
