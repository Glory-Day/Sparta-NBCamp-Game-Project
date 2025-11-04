using Backend.Object.Character.Player;
using UnityEngine;

namespace Script.Object.Character.Player
{
    public class PlayerCharacterComposer : MonoBehaviour
    {
        [field: Header("Component References")]
        [field: SerializeField] public AdvancedActionController AdvancedActionController { get; private set; }
        [field: SerializeField] public CameraDistanceRaycaster CameraDistanceRaycaster { get; private set; }
        [field: SerializeField] public EnemyCharacterFinder EnemyCharacterFinder { get; private set; }
        [field: SerializeField] public PlayerPerspectiveController PerspectiveController { get; private set; }
        [field: SerializeField] public PlayerAnimationController AnimationController { get; private set; }
        [field: SerializeField] public PlayerMovementController MovementController { get; private set; }
        [field: SerializeField] public ThirdPersonCameraController ThirdPersonCameraController { get; private set; }
    }
}
