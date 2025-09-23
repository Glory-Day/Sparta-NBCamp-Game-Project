using System.Collections;
using Backend.Object.Management;
using Backend.Util.Data.ActionDatas;
using UnityEngine;

namespace Backend.Object.Character.Enemy.Boss.Skill
{
    public class NineTailBeastSkillS0204 : BossSkillBase
    {
        public void SpawnProjectileS0204()
        {
            foreach (var projectileTransform in projectileTransform)
            {
                ObjectPoolManager.SpawnPoolObject(projectilePrefab, projectileTransform.position, projectileTransform.rotation, projectileTransform);
            }
        }
    }
}
