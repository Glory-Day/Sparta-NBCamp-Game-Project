using UnityEngine;

namespace Backend.Object.Character.Player
{
    public class ThirdPersonCameraController : CameraController
    {
        [Header("Controller Reference")]
        public AdvancedActionController controller;
        
        [Header("3th Person Settings")]
        //The general rate at which the camera turns toward the movement direction;
        public float turningTowardSpeed = 120f;
        
        //The maximum expected movement speed of this game object;
        //This value should be set to the maximum movement speed achievable by this instance;
        //The closer the current movement speed is to 'maximumMovementSpeed', the faster the camera will turn;
        //As a result, if the instance moves slower (i.e. "walking" instead of "running", in case of a character), the camera will turn slower as well.
        public float maximumMovementSpeed = 7f;

        // Whether the camera turns towards the controller's movement direction.
        public bool isTurningToward = true;

        protected override void SetUp()
        {
            if (controller == null)
            {
                Debug.LogWarning("No controller reference has been assigned to this script.", gameObject);
            }
        }

        protected override void HandleCameraRotation()
        {
            // Execute normal camera rotation code.
            base.HandleCameraRotation();

            if (controller == null)
            {
                return;
            }

            if (isTurningToward == false || controller == null)
            {
                return;
            }
            
            // Get controller velocity.
            var velocity = controller.Velocity;

            RotateTowardsVelocity(velocity, turningTowardSpeed);
        }
        
        /// <summary>
        /// Rotate camera toward direction, at the rate of _speed, around the upwards vector of this instance.
        /// </summary>
        private void RotateTowardsVelocity(Vector3 velocity, float speed)
        {
            //Remove any unwanted components of direction.
            velocity = VectorMath.RemoveDotVector(velocity, GetUpDirection());

            // Calculate angle difference of current direction and new direction.
            var angle = VectorMath.GetAngle(GetFacingDirection(), velocity, GetUpDirection());
            
            // Calculate sign of angle.
            var sign = Mathf.Sign(angle);

            // Calculate final angle difference.
            var result = sign * Mathf.Abs(angle / 90f) * speed * Time.deltaTime;

            // If angle is greater than 90 degrees, recalculate final angle difference.
            if (Mathf.Abs(angle) > 90f)
            {
                result = Time.deltaTime * speed * sign * (Mathf.Abs(180f - Mathf.Abs(angle)) / 90f);
            }

            // Check if calculated angle overshoots.
            if (Mathf.Abs(result) > Mathf.Abs(angle))
            {
                result = angle;
            }

            // Take movement speed into account by comparing it to maximum movement speed.
            result *= Mathf.InverseLerp(0f, maximumMovementSpeed, velocity.magnitude);

            SetRotationAngles(GetCurrentXAngle(), GetCurrentYAngle() + result);
        }
    }
}
