using System.Collections;
using Backend.Object.Management;
using Backend.Util.Data.ActionDatas;
using UnityEngine;

namespace Backend.Object.Character.Enemy.Boss.Skill
{
    public class NineTailBeastSkillS0201 : BossSkillBase
    {
        public void SpawnProjectile()
        {
            ObjectPoolManager.SpawnPoolObject(projectilePrefab, projectileTransform[0].position, projectileTransform[0].rotation, projectileTransform[0]);
        }
    }
}
