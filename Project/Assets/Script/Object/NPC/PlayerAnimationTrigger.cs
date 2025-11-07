using System;
using Backend.Object.Management;
using Backend.Util.Debug;
using Backend.Util.Input;
using Backend.Util.Management;
using Script.Object.Character.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Backend.Object.NPC
{
    public class PlayerAnimationTrigger : MonoBehaviour
    {
        [field: Header("Debug Information")]
        [field: SerializeField] public PlayerCharacterComposer Composer { get; set; }

        private PlayerCharacterSpawner _spawner;

        private NPCControls _controls;

        private void Awake()
        {
            _spawner = GetComponent<PlayerCharacterSpawner>();
        }

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
            DataManager.UserData.SceneIndex = SceneManager.CurrentSceneIndex;
            DataManager.UserData.SpawnerIndex = _spawner.Identifier;
            DataManager.SaveAllData();

            Composer.AdvancedActionController.Rest();
        }

        private void StandUp(InputAction.CallbackContext context)
        {
            Composer.AdvancedActionController.StandUp();
        }
    }
}
