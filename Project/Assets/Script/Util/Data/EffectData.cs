using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Backend.Util.Data
{
    [System.Serializable, CreateAssetMenu(fileName = "EffectData", menuName = "EffectData")]
    public class EffectData : ScriptableObject
    {
        public string Id;
        public int Count;
        public GameObject Prefab;
    }
}
