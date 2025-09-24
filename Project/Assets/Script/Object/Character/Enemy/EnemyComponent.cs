using Backend.Object.Character.Enemy.Boss;

namespace Backend.Object.Character.Enemy
{
    public class EnemyComponent
    {
        public EnemyStatus Status { get; set; }
        public EnemyAnimationController AnimationController { get; set; }
        public EnemyMovementController MovementController { get; set; }
        public EnemyCombatController CombatController { get; set; }
    }
}
