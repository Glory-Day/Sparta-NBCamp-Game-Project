using UnityEngine;

namespace Backend.Object.Character.Player
{
    //This camera input class is an example of how to get input from a connected mouse using Unity's default input system;
    //It also includes an optional mouse sensitivity setting;
    public class MouseInputSystem : MonoBehaviour
    {
        #region SERIALIZABLE PROPERTIES API

        [field: Header("IO System Settings")]
        [field: SerializeField] public string MouseHorizontalAxis { get; private set; } = "Mouse X";

        [field: SerializeField] public string MouseVerticalAxis { get; private set; } = "Mouse Y";

        [field: Header("Input Settings")]
        [field: SerializeField] public bool IsHorizontalInputInverted { get; private set; }
        [field: SerializeField] public bool IsVerticalInputInverted { get; private set; }

        [field: Header("Controller Settings")]
        [field: Tooltip("Use this value to fine-tune mouse movement. All mouse input will be multiplied by this value.\n\n" +
                        "해당 값을 사용하여 마우스 움직임을 미세 조정할 수 있다. 모든 마우스 입력값은 이 값으로 곱해진다.")]
        [field: SerializeField] public float Multiplier { get; private set; } = 0.01f;

        #endregion

        private bool _isEnabled = true;

        public void Enable()
        {
            _isEnabled = true;
        }

        public void Disable()
        {
            _isEnabled = false;
        }

        public float GetHorizontalCameraInput()
        {
            if (_isEnabled == false)
            {
                return 0f;
            }

            // Get raw mouse input.
            var input = Input.GetAxisRaw(MouseHorizontalAxis);

            // Since raw mouse input is already time-based, we need to correct for this before passing the input to the camera controller.
            if (Time.timeScale > 0f && Time.deltaTime > 0f)
            {
                input /= Time.deltaTime;
                input *= Time.timeScale;
            }
            else
            {
                input = 0f;
            }

            // Apply mouse sensitivity.
            input *= Multiplier;

            // Invert input.
            if (IsHorizontalInputInverted)
            {
                input *= -1f;
            }

            return input;
        }

        public float GetVerticalCameraInput()
        {
            if (_isEnabled == false)
            {
                return 0f;
            }

            // Get raw mouse input.
            var input = -Input.GetAxisRaw(MouseVerticalAxis);

            // Since raw mouse input is already time-based, we need to correct for this before passing the input to the camera controller.
            if (Time.timeScale > 0f && Time.deltaTime > 0f)
            {
                input /= Time.deltaTime;
                input *= Time.timeScale;
            }
            else
            {
                input = 0f;
            }

            // Apply mouse sensitivity.
            input *= Multiplier;

            // Invert input.
            if (IsVerticalInputInverted)
            {
                input *= -1f;
            }

            return input;
        }
    }
}
