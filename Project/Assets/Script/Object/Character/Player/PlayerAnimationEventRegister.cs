using Backend.Util.Debug;
using UnityEngine;

namespace Backend.Object.Character.Player
{
    public class PlayerAnimationEventRegister : MonoBehaviour
    {
        private DamageSender _damageSender;

        private void Awake()
        {
            _damageSender = GetComponentInChildren<DamageSender>();
        }

        public void OnDetectionStarted()
        {
            Debugger.LogProgress();

            _damageSender.StartDetection();
        }

        public void OnDetectionStopped()
        {
            Debugger.LogProgress();

            _damageSender.StopDetection();
        }
    }
}
