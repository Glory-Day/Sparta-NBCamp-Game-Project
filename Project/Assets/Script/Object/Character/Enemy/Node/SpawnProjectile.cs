using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnProjectile : ActionNode
{
    public GameObject Projectile;
    public Transform[] ProjectileSpawnPoint;

    protected override void Start() { }
    protected override void Stop() { }
    protected override State OnUpdate()
    {
        return State.Success;
    }
}
