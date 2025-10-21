using System.Collections;
using System.Collections.Generic;
using Backend.Object.Management;
using Backend.Util.Data;
using UnityEngine;

namespace Backend.Object.Character
{
    public class VisualEffectPlayer : MonoBehaviour
    {
        public List<EffectData> EffectData;

        private Coroutine _effectCoroutine;

        private void Awake()
        {
            PreLoad(EffectData);
        }

        private void PreLoad(List<EffectData> effectData)
        {
            for (int i = 0; i < effectData.Count; i++)
            {
                ObjectPoolManager.CreatePoolObject(effectData[i].Prefab, effectData.Count, transform);
            }
        }

        public ParticleSystem Play(int id, Vector3 position, Quaternion rotation)
        {
            if (_effectCoroutine != null)
            {
                return null;
            }

            var clone = ObjectPoolManager.SpawnPoolObject(EffectData[id].Prefab, position, rotation, transform);
            var effect = clone.GetComponentInChildren<ParticleSystem>();
            effect.Play();
            _effectCoroutine = StartCoroutine(Release(id, effect));

            return effect;
        }

        private IEnumerator Release(int id, ParticleSystem effect)
        {
            yield return new WaitUntil(() => effect.isPlaying == false);

            ObjectPoolManager.Release(effect.gameObject);
        }

        public void Stop(int id, ParticleSystem effect)
        {
            _effectCoroutine = null;

            ObjectPoolManager.Release(effect.gameObject);
            effect.Stop();
        }
    }
}
