using System.Collections;
using System.Collections.Generic;
using Backend.Object.Management;
using UnityEngine;

namespace Backend.Object.Character.Enemy.Node
{
    public class SpawnProjectile : ActionNode
    {
        public GameObject Projectile;
        public Vector3[] ProjectileSpawnPoint;

        public float Delay = 0.2f;
        private float _timer;
        private int _spawnCount;

        protected override void Start()
        {
            _timer = 0f;
            _spawnCount = 0;
        }
        protected override void Stop() { }
        protected override State OnUpdate()
        {
            _timer += Time.deltaTime;

            if (_timer >= Delay && _spawnCount < ProjectileSpawnPoint.Length)
            {
                var spawnPoint = agent.MovementController.transform.TransformPoint(ProjectileSpawnPoint[_spawnCount]);
                ObjectPoolManager.SpawnPoolObject(Projectile, spawnPoint, Quaternion.identity, agent.MovementController.transform);

                _spawnCount++;
                _timer = 0f;
            }

            if (_spawnCount >= ProjectileSpawnPoint.Length)
            {
                return State.Success;
            }

            return State.Running;
        }
    }
}
