using System;
using UnityEngine;

namespace Backend.Util.Json.Data
{
    [Serializable]
    public class SettingData
    {
        [SerializeField] public float MasterAudioVolume { get; set; } = 0.5f;
        [SerializeField] public float EffectAudioVolume { get; set; } = 0.5f;
        [SerializeField] public float VoiceAudioVolume { get; set; } = 0.5f;
    }
}
