using UnityEngine;

namespace Backend.Object.Character.Player
{
	//This character movement input class is an example of how to get input from a keyboard to control the character;
    public class CharacterKeyboardInput : MonoBehaviour
    {
        [Header("IO System Settings")]
		public string horizontalInputAxis = "Horizontal";
		public string verticalInputAxis = "Vertical";
		public KeyCode jumpKey = KeyCode.Space;

        [Tooltip("If this is enabled, Unity's internal input smoothing is bypassed.\n\n" +
                 "이 옵션이 활성화되면 Unity의 내부 입력 스무딩이 우회된다.")]
		public bool useRawInput = true;

        private void Update()
        {
            var isPressed = Input.GetKey(jumpKey);

            switch (IsJumpKeyPressed)
            {
                case false when isPressed == true:
                    WasJumpKeyPressed = true;
                    break;
                case true when isPressed == false:
                    WasJumpKeyReleased = true;
                    IsJumpLocked = false;
                    break;
            }

            IsJumpKeyPressed = isPressed;
        }
        
        public float GetHorizontalMovementInput()
        {
            return useRawInput ? Input.GetAxisRaw(horizontalInputAxis) : Input.GetAxis(horizontalInputAxis);
        }

		public float GetVerticalMovementInput()
        {
            return useRawInput ? Input.GetAxisRaw(verticalInputAxis) : Input.GetAxis(verticalInputAxis);
        }
        
        public float LastJumpPressedTime { get; set; }
        
        //Jump key variables;
        public bool IsJumpLocked { get; set; }
        
        public bool WasJumpKeyPressed  { get; set; }
        
        public bool WasJumpKeyReleased  { get; set; }
        
        public bool IsJumpKeyPressed  { get; set; }
    }
}
