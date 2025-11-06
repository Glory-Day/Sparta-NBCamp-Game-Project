using System;
using Backend.Util.Debug;
using Backend.Util.Input;
using Script.Object.Character.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Backend.Object.NPC
{
    public class PlayerAnimationTrigger : MonoBehaviour
    {
        [Header("Debug Information")]
        [SerializeField] private PlayerCharacterComposer composer;

        private NPCControls _controls;

        public void EnableControls()
        {
            Debugger.LogProgress();

            _controls = new NPCControls();
            _controls.Enable();
            _controls.Campfire.Rest.performed += Rest;
            _controls.Campfire.StandUp.performed += StandUp;
        }

        public void DisableControls()
        {
            Debugger.LogProgress();

            _controls.Campfire.Rest.performed -= Rest;
            _controls.Campfire.StandUp.performed -= StandUp;
            _controls.Disable();
            _controls = null;
        }

        private void Rest(InputAction.CallbackContext context)
        {
            composer.AdvancedActionController.Rest();
        }

        private void StandUp(InputAction.CallbackContext context)
        {
            composer.AdvancedActionController.StandUp();
        }
    }
}
