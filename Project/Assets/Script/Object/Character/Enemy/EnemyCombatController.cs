﻿using System.Collections;
using System.Collections.Generic;
using Backend.Object.Character;
using Backend.Util;
using Backend.Util.Data.ActionDatas;
using UnityEngine;

public class EnemyCombatController : MonoBehaviour
{
    [field: SerializeField] public ActionBossData[] ActionDatas { get; private set; }
    [field: SerializeField] public ActionBossData ActionData { get; set; }
    [SerializeField] private WeaponController _weapon;
    [SerializeField] private WeaponController[] _weapons;
    public readonly Dictionary<string, CoolDownTimer> ActionCoolTimer = new(); // 스킬 쿨다운 타이머 딕셔너리

    public void Start()
    {
        foreach (var action in ActionDatas)
        {
            ActionCoolTimer.Add(action.ID, new CoolDownTimer(action.CoolDown));
        }
    }
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
