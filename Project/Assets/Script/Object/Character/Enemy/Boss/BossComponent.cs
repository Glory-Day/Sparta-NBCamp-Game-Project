using System.Collections;
using System.Collections.Generic;
using Backend.Object.Character.Enemy;
using Backend.Object.Character.Enemy.Boss;
using UnityEngine;

public class BossComponent
{
    public EnemyStatus Status { get; set; }
    public EnemyAnimationController AnimationController { get; set; }
    public EnemyMovementController MovementController { get; set; }
    public EnemyCombatController CombatController { get; set; }
}
