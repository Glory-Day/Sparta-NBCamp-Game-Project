using System.Collections;
using System.Collections.Generic;
using Backend.Object.Character;
using Backend.Util.Data.ActionDatas;
using UnityEngine;

public class EnemyCombatController : MonoBehaviour
{
    [field: SerializeField] public ActionBossData[] ActionDatas { get; private set; }
    [field: SerializeField] public ActionBossData ActionData { get; set; }
    [SerializeField] private WeaponController _weapon;
    [SerializeField] private WeaponController[] _weapons;

    public void StartAttack()
    {
        _weapon.StartAttack(ActionData.Damage);
    }

    public void StartAttackOf(int attackNum)
    {
        _weapon.StartAttack(ActionDatas[attackNum].Damage);
    }

    public void EndAttack()
    {
        if(_weapon != null)
        {
            _weapon.StopAttack();
        }
    }

    public void SetWeapon(int weaponNum)
    {
        _weapon = _weapons[weaponNum];
    }
}
