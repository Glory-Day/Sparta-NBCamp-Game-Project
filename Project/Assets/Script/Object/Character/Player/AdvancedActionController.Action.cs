using System;
using Backend.Util.Debug;
using Backend.Util.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Backend.Object.Character.Player
{
    public partial class AdvancedActionController
    {
        private PlayerControls _actions;
        private Vector3 _facing = Vector3.zero;
        private Vector3 _direction = Vector3.zero;

        private bool _isRolling;
        private bool _isRollButtonBuffered;

        private float _lastJumpButtonUsed;

        private int _combatIndex = -1;
        private bool _isAttacking;
        private bool _isAttackButtonBuffered;

        public event Action<Vector3> OnJump;
        public event Action<Vector3> OnLand;

        private void Move(InputAction.CallbackContext context)
        {
            _direction = context.ReadValue<Vector3>().normalized;
            _facing = _direction == Vector3.zero ? _facing : _direction;
        }

        private void Jump(InputAction.CallbackContext context)
        {
            if (_state != State.Grounded)
            {
                return;
            }

            if (Time.time < _lastJumpButtonUsed + jumpDuration)
            {
                return;
            }

            TranslateToAirborne();

            _momentum = ConvertMomentumToWorldSpace();

            // Add jump force to momentum.
            _momentum += transform.up * jumpSpeed;

            // Set jump start time.
            _lastJumpButtonUsed = Time.time;

            OnJump?.Invoke(_momentum);

            _momentum = ConvertMomentumToLocalSpace();

            _state = State.Jumping;
        }

        private void Roll(InputAction.CallbackContext context)
        {
            Debugger.LogProgress();

            if (_status.IsUsingStaminaAvailable() == false)
            {
                return;
            }

            switch (_state)
            {
                case State.Grounded when _state != State.Rolling:
                    _animationController.SetAnimationBoolean("Is Rolling", true);
                    break;
                case State.Rolling:
                    _isRollButtonBuffered = true;
                    break;
                case State.Sliding:
                case State.Falling:
                case State.Rising:
                case State.Jumping:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Attack(InputAction.CallbackContext context)
        {
            _animationController.SetAnimationTrigger("Attack");
        }

        private void Stop(InputAction.CallbackContext context)
        {
            _direction = Vector3.zero;
        }

        public void OnRollingStateEntered()
        {
            Debugger.LogProgress();

            _state = State.Rolling;
            _isRollButtonBuffered = false;

            _status.UseStamina();

            _movementController.IsColliderEnabled = false;

            _actions.Movement.Move.Disable();
            _actions.Movement.Jump.Disable();
            _actions.Movement.Attack.Disable();

            _direction = _facing;
        }

        public void OnRollingStateExited()
        {
            Debugger.LogProgress();

            _state = State.Grounded;
            _direction = Vector3.zero;

            _movementController.IsColliderEnabled = true;

            _actions.Movement.Move.Enable();
            _actions.Movement.Jump.Enable();
            _actions.Movement.Attack.Enable();

            _animationController.SetAnimationBoolean("Is Rolling", false);

            if (_isRollButtonBuffered)
            {
                _animationController.SetAnimationBoolean("Is Rolling", true);
            }
        }

        public void OnAttackingStateEntered()
        {
            Debugger.LogProgress();

            _state = State.Attacking;

            _actions.Movement.Move.Disable();
            _actions.Movement.Jump.Disable();
            _actions.Movement.Roll.Disable();

            _direction = Vector3.zero;
        }

        public void OnAttackingStateExited()
        {
            Debugger.LogProgress();

            _state = State.Grounded;
            _direction = Vector3.zero;

            _actions.Movement.Move.Enable();
            _actions.Movement.Jump.Enable();
            _actions.Movement.Roll.Enable();
        }

        public void OnDamagedStateEntered()
        {
            Debugger.LogProgress();

            _actions.Movement.Move.Disable();
            _actions.Movement.Jump.Disable();
            _actions.Movement.Roll.Disable();
            _actions.Movement.Attack.Disable();
        }

        public void OnDamagedStateExited()
        {
            Debugger.LogProgress();

            _actions.Movement.Move.Enable();
            _actions.Movement.Jump.Enable();
            _actions.Movement.Roll.Enable();
            _actions.Movement.Attack.Enable();

            _animationController.SetAnimationFloat("Damage", 0f);
        }
    }
}
