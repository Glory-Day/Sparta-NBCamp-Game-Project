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
                    if (IsMoving)
                    {
                        // 입력(Direction[0])을 카메라 기준 월드 방향으로 변환해서 저장
                        if (CameraTransform != null)
                        {
                            var v = Vector3.ProjectOnPlane(CameraTransform.forward, transform.up).normalized;
                            var h = Vector3.ProjectOnPlane(CameraTransform.right, transform.up).normalized;

                            var input = Direction[0];
                            var worldDir = (h * input.x) + (v * input.z);

                            Direction[1] = worldDir.sqrMagnitude > 0f ? worldDir.normalized : transform.forward;
                        }
                        else
                        {
                            // 카메라가 없으면 로컬 forward를 사용
                            Direction[1] = transform.forward;
                        }
                    }
                    else
                    {
                        // 정지 상태에서의 롤은 카메라(또는 시점)의 forward를 기준으로 롤
                        var fwd = Composer.PerspectiveController.Forward;
                        fwd.y = 0f;
                        Direction[1] = fwd.sqrMagnitude > 0f ? fwd.normalized : transform.forward;
                    }

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

                    // 동일한 방식으로 공격 시에도 Direction[1]을 월드 방향으로 저장
                    if (IsMoving)
                    {
                        if (CameraTransform != null)
                        {
                            var v = Vector3.ProjectOnPlane(CameraTransform.forward, transform.up).normalized;
                            var h = Vector3.ProjectOnPlane(CameraTransform.right, transform.up).normalized;

                            var input = Direction[0];
                            var worldDir = (h * input.x) + (v * input.z);

                            Direction[1] = worldDir.sqrMagnitude > 0f ? worldDir.normalized : transform.forward;
                        }
                        else
                        {
                            Direction[1] = transform.forward;
                        }
                    }
                    else
                    {
                        var fwd = Composer.PerspectiveController.Forward;
                        fwd.y = 0f;
                        Direction[1] = fwd.sqrMagnitude > 0f ? fwd.normalized : transform.forward;
                    }

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
