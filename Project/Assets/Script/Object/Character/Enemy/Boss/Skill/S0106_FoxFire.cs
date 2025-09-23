using System.Collections;
using System.Collections.Generic;
using Backend.Object.Character;
using Backend.Object.Character.Enemy.Boss;
using Backend.Object.Character.Enemy.Boss.Skill;
using Backend.Object.Management;
using Backend.Util.Data.ActionDatas;
using Backend.Util.Debug;
using Unity.Mathematics;
using UnityEngine;

namespace Backend.Object.Character.Enemy.Boss.Skill
{
    public class S0106_FoxFire : BossSkillBase
    {
        [field: SerializeField] public GameObject FoxFire { get; private set; }
        public Vector3[] FirePos = new Vector3[3];
        public Transform Target; //임시

        protected override IEnumerator ExecuteSkillLogic(EnemyAnimationController animController, ActionBossData data)
        {
            Debugger.LogSuccess("여우불 발동");

            for (int i = 0; i < 3; i++)
            {
                if (FoxFire == null)
                {
                    Debugger.LogError("FoxFire is null.");
                    yield break;
                }

                Vector3 spawnPos = transform.TransformPoint(FirePos[i]);
                var fireball = ObjectPoolManager.SpawnPoolObject(FoxFire, spawnPos, Quaternion.identity, null);

                if (fireball.TryGetComponent<BossProjectile>(out var projectile))
                {
                    float damage = SkillData.Damage * 3f;
                    projectile.Initialize(Target, damage);
                }
                else
                {
                    Debugger.LogError("projectile is null");
                }

                yield return new WaitForSeconds(0.4f);
            }
            yield return null;
        }
    }
}
