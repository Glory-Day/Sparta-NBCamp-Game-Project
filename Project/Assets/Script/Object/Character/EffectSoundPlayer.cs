using System.Collections;
using System.Collections.Generic;
using Backend.Util.Debug;
using Backend.Util.Management;
using UnityEngine;
using static Unity.VisualScripting.Member;

namespace Backend.Object.Character
{
    [RequireComponent(typeof(AudioSource))]
    public class EffectSoundPlayer : MonoBehaviour
    {
        [SerializeField] private List<AudioClip> _clips;
        [SerializeField] private float _maximumDistance;

        private AudioSource _source;

        private void Awake()
        {
            _source = GetComponent<AudioSource>();

            _source.outputAudioMixerGroup = SoundManager.EffectAudioMixerGroup;
            _source.spatialBlend = 1f;
            _source.rolloffMode = AudioRolloffMode.Linear;
            _source.maxDistance = _maximumDistance;
        }

        public void Play(int index)
        {
            if (index < 0 || index >= _clips.Count)
            {
                Debugger.LogError($"유효한 인덱스 번호가 아닙니다 -> {index}");
                return;
            }
            //_source.Stop();
            //_source.clip = _clips[index];
            _source.PlayOneShot(_clips[index]);
        }

#if UNITY_EDITOR
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                Play(0);
            }
        }
#endif
    }
}
