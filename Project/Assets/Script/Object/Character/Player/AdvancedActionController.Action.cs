using System;
using Backend.Util.Debug;
using Backend.Util.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Backend.Object.Character.Player
{
    public partial class AdvancedActionController
    {
        private bool _isRolling;

        private int _combatIndex = -1;
        private bool _isAttacking;
        private bool _isAttackButtonBuffered;

        public event Action<Vector3> OnLand;

        private void OnMoveStarted(InputAction.CallbackContext context)
        {
            IsMoving = true;
            Composer.AnimationController.SetAnimationBoolean("Is Moving", true);
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            Direction[0] = context.ReadValue<Vector3>().normalized;
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            IsMoving = false;
            Composer.AnimationController.SetAnimationBoolean("Is Moving", false);

            Direction[0] = Vector3.zero;
        }

        private void OnRollPerformed(InputAction.CallbackContext context)
        {
            if (_status.IsUsingStaminaAvailable(0) == false)
            {
                return;
            }

            switch (State)
            {
                case State.Grounded:
                case State.Attacking when IsButtonBufferable:
                case State.Rolling when IsButtonBufferable:
                case State.Stunned when IsButtonBufferable:
                {
                    Debug.Log("Now rolling...");

                    //TODO: Fix this code. This code in comment for test.
                    _status.UseStamina(0);

                    Direction[1] = IsMoving ? Direction[0] : Composer.PerspectiveController.Forward;
                    IsButtonBufferable = false;

                    Composer.AnimationController.SetAnimationTrigger("Rolled");

                    break;
                }
                case State.Sliding:
                case State.Falling:
                case State.Rising:
                default:
                    break;
            }
        }

        private void OnAttackPerformed(InputAction.CallbackContext context)
        {
            if (_status.IsUsingStaminaAvailable(1) == false)
            {
                return;
            }

            switch (State)
            {
                case State.Grounded:
                case State.Attacking when IsButtonBufferable:
                case State.Rolling when IsButtonBufferable:
                case State.Stunned when IsButtonBufferable:
                {
                    //TODO: Fix this code. This code in comment for test.
                    _status.UseStamina(1);

                    Direction[1] = IsMoving ? Direction[0] : Composer.PerspectiveController.Forward;
                    IsButtonBufferable = false;

                    Composer.AnimationController.SetAnimationTrigger("Attacked");
                    break;
                }
                case State.Sliding:
                case State.Falling:
                case State.Rising:
                default:
                    break;
            }
        }

        public PlayerControls Controls { get; private set; }

        public Vector3[] Direction { get; set; } = new Vector3[2];

        public Vector3 Forward { get; set; }

        public bool IsMoving { get; private set; }

        public bool IsAttackButtonBufferable { get; set; }

        public bool IsRollButtonBuffered { get; set; }

        public bool IsButtonBufferable { get; set; }
    }
}
