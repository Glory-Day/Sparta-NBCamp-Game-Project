using UnityEngine;

namespace Backend.Util
{
    public class CoolDownTimer
    {
        private float _time;
        private float _duration;

        public CoolDownTimer(float time)
        {
            _duration = time;
        }

        public void Start()
        {
            _time = UnityEngine.Time.time + _duration;
        }

        public void Stop()
        {
            _time = 0f;
        }

        public float RemainingTime => Mathf.Max(0f, _time - UnityEngine.Time.time);

        public bool IsCoolingDown => UnityEngine.Time.time < _time;
    }
}
