using UnityEngine;

namespace Backend.Object.Character.Player
{
    public class PlayerAnimationEventRegistry : MonoBehaviour
    {
        private AdvancedActionController _actionController;

        private DamageSender _sender;

        private void Awake()
        {
            _actionController = GetComponentInParent<AdvancedActionController>();

            _sender = GetComponentInChildren<DamageSender>();
        }

        private void OnAttackStarted()
        {
            _sender.StartDetection();
        }

        private void OnAttackStopped()
        {
            _sender.StopDetection();
        }

        private void OnRollButtonBufferedValid()
        {
            _actionController.IsRollButtonBufferable = true;
        }

        private void OnColliderEnabled()
        {
            _actionController.EnableCollider();
        }

        private void OnColliderDisabled()
        {
            _actionController.DisableCollider();
        }
    }
}
