using System;
using Backend.Object.Character.Enemy.Boss.Skill;
using Backend.Util.Data.ActionDatas;
using Backend.Util.Debug;
using Ironcow.Synapse;
using UnityEngine;

namespace Backend.Object.Character.Enemy.Boss
{
    public partial class NineTailHuman : MonoBehaviour
    {
    //    // 스킬이 실행 중인지 여부를 나타내는 노드
    //    public eNodeState IsSkillExecuting(DataContext context)
    //    {
    //        if (_isSkillExecuting)
    //        {
    //            return eNodeState.failure;
    //        }
    //        else
    //        {
    //            return eNodeState.success;
    //        }
    //    }

    //    public eNodeState CoolDownCheck(DataContext context)
    //    {
    //        if (_isSkillExecuting)
    //        {
    //            return eNodeState.failure;
    //        }

    //        foreach (string cool in _cooltimer.Keys)
    //        {
    //            if (!_cooltimer[cool].IsCoolingDown)
    //            {
    //                _usableSkills.Add(_skillCache[cool]);
    //            }
    //        }
    //        if (_usableSkills.Count <= 0)
    //        {
    //            return eNodeState.failure;
    //        }
    //        else
    //        {
    //            return eNodeState.success;
    //        }
    //    }

    //    // 우선순위가 가장 높은 스킬 설정
    //    public eNodeState SkillPriorityCheck(DataContext context)
    //    {
    //        // 스킬 리스트 랜덤으로 섞기
    //        //_usableSkills = _usableSkills.OrderBy(x => Random.value).ToList();

    //        float totalWeight = 0f; // 전체 가중치 합산
    //        foreach (var skill in _usableSkills)
    //        {
    //            if (skill.AttackWeight == 0)
    //            {
    //                _selectedSkill = skill;
    //                return eNodeState.success;
    //            }

    //            totalWeight += skill.AttackWeight;
    //        }

    //        float randomPoint = UnityEngine.Random.Range(0, totalWeight); // 0부터 전체 가중치 사이의 랜덤 값 생성
    //        foreach (var skill in _usableSkills)
    //        {
    //            if (randomPoint <= skill.AttackWeight) // Random < 40
    //            {
    //                _selectedSkill = skill;
    //                return eNodeState.success;
    //            }
    //            randomPoint -= skill.AttackWeight; // 가중치만큼 감소 Random - 40  
    //        }

    //        return eNodeState.failure;
    //    }

    //    // 랜덤으로 스킬 선택
    //    public eNodeState SelectRandomSkill(DataContext context)
    //    {
    //        Debug.Log($"스킬 선택: {_selectedSkill.Name} \n플레이어와 거리: {_movementController.Distance}");

    //        // 사용후 초기화 및 쿨다운 시작
    //        _usableSkills.Clear();
    //        if (_selectedSkill.CoolDown > 0)
    //        {
    //            _cooltimer[_selectedSkill.ID].Start();
    //        }
    //        return eNodeState.success;
    //    }

    //    //public eNodeState ExecuteSkill(DataContext context)
    //    //{
    //    //    if (_selectedSkill == null)
    //    //    {
    //    //        _usableSkills.Clear();
    //    //        return eNodeState.failure;
    //    //    }

    //    //    // 스킬이 이미 코루틴으로 실행중이면 상태 보고
    //    //    if (_skillCoroutine == null)
    //    //    {
    //    //        // 매칭되는 BossSkillBase 찾기
    //    //        BossSkillBase targetSkill = null;
    //    //        foreach (var skill in _bossSkills)
    //    //        {
    //    //            if (skill.SkillData.ID == _selectedSkill.ID)
    //    //            {
    //    //                targetSkill = skill;
    //    //                break;
    //    //            }
    //    //        }

    //    //        // 스킬이 없으면 실패로 정리
    //    //        if (targetSkill == null)
    //    //        {
    //    //            Debug.LogWarning($"ExecuteSkill: _bossSkills에서 선택된 스킬({_selectedSkill.ID})을 찾을 수 없습니다.");
    //    //            _usableSkills.Clear();
    //    //            _selectedSkill = null;
    //    //            _isSkillExecuting = false; // 안전하게 false로
    //    //            return eNodeState.failure;
    //    //        }

    //    //        // 안전한 구독/해제 패턴
    //    //        Action onEnd = null;
    //    //        onEnd = () =>
    //    //        {
    //    //            _isSkillExecuting = false;
    //    //            _skillCoroutine = null;
    //    //            _selectedSkill = null;
    //    //            targetSkill.OnSkillEnd -= onEnd; // 구독 해제
    //    //        };

    //    //        targetSkill.OnSkillEnd += onEnd;

    //    //        // 코루틴 시작 후에야 실행 상태를 true로 설정
    //    //        _skillCoroutine = StartCoroutine(targetSkill.ExecuteSkill(_animationController, _selectedSkill));
    //    //        _isSkillExecuting = true;

    //    //        Debug.Log($"스킬 실행: {_selectedSkill.Name}");

    //    //        // 사용 후 정리 및 쿨다운 시작 (쿨다운 중복 시작되는지 확인해서 하나로 통일하세요)
    //    //        _usableSkills.Clear();
    //    //        if (_selectedSkill.CoolDown > 0)
    //    //        {
    //    //            _cooltimer[_selectedSkill.ID].Start();
    //    //        }
    //    //    }

    //    //    return _isSkillExecuting ? eNodeState.running : eNodeState.success;
    //    //}

    //    // 선택된 스킬을 실행 
    //    public eNodeState ExecuteSkill(DataContext context)
    //    {
    //        if (_selectedSkill == null)
    //        {
    //            _usableSkills.Clear();
    //            return eNodeState.failure;
    //        }

    //        if (_skillCoroutine == null)
    //        {
    //            foreach (var skill in _bossSkills)
    //            {
    //                if (skill.SkillData.ID == _selectedSkill.ID)
    //                {
    //                    skill.OnSkillEnd += () =>
    //                    {
    //                        _isSkillExecuting = false;
    //                        _skillCoroutine = null;
    //                        _selectedSkill = null;
    //                    };

    //                    _isSkillExecuting = true;
    //                    _skillCoroutine = StartCoroutine(skill.ExecuteSkill(_animationController, _selectedSkill));

    //                    Debug.Log($"스킬 실행: {_selectedSkill.Name}");


    //                    _usableSkills.Clear();
    //                    if (_selectedSkill.CoolDown > 0)
    //                    {
    //                        _cooltimer[_selectedSkill.ID].Start();
    //                    }
    //                    break;
    //                }
    //            }
    //        }

    //        if (_isSkillExecuting)
    //        {
    //            return eNodeState.running;
    //        }
    //        else
    //        {
    //            return eNodeState.success;
    //        }
    //    }

    //    public eNodeState IsExecutingCheck(DataContext context)
    //    {
    //        _isSkillExecuting = false;
    //        return eNodeState.success;
    //    }

    //    public eNodeState SkillDistanceCheck(DataContext context)
    //    {
    //        for (int i = _usableSkills.Count - 1; i >= 0; i--)
    //        {
    //            ActionBossData skill = _usableSkills[i];

    //            int index = (int)skill.ActionRange;

    //            float maxDis = _status.BossStatus.AttackRange[index];
    //            float minDis = index == 0 ? 0f : _status.BossStatus.AttackRange[index - 1];

    //            bool isInRange = _movementController.Distance > minDis && _movementController.Distance <= maxDis;

    //            if (!isInRange)
    //            {
    //                _usableSkills.RemoveAt(i);
    //            }
    //        }

    //        if (_usableSkills.Count <= 0)
    //        {
    //            return eNodeState.failure;
    //        }
    //        else
    //        {
    //            return eNodeState.success;
    //        }
    //    }

    //    public eNodeState TurnToPlayer(DataContext context)
    //    {
    //        Debugger.LogSuccess("회전");
    //        _movementController.SetLerpRotation();
    //        return eNodeState.success;
    //    }

    //    public eNodeState DeathState(DataContext context)
    //    {
    //        Debugger.LogSuccess("사망");
    //        //if (CurrentHp <= 0)
    //        //{
    //        //    return eNodeState.success;
    //        //}
    //        return eNodeState.failure;
    //    }

    //    public eNodeState Strafe(DataContext context)
    //    {
    //        if (_isSkillExecuting) //스킬 사용시에는 true임.
    //        {
    //            return eNodeState.failure;
    //        }

    //        if (!_movementController.IsStrafing())
    //        {
    //            _movementController.StartStrafe();
    //        }

    //        if(_movementController.IsStrafing())
    //        {
    //            return eNodeState.running;
    //        }
    //        else
    //        {
    //            return eNodeState.success;
    //        }
    //    }

    //    public eNodeState IsCloseRange(DataContext context)
    //    {
    //        Debugger.LogMessage($"근접 체크일때 _isSkillExecuting : {_isSkillExecuting}");
    //        if (_movementController.Distance <= _status.BossStatus.AttackRange[0])
    //        {
    //            return eNodeState.success;
    //        }

    //        return eNodeState.failure;
    //    }

    //    public eNodeState IsMidRange(DataContext context)
    //    {
    //        Debugger.LogMessage($"중거리 체크일때 _isSkillExecuting : {_isSkillExecuting}");
    //        if (_movementController.Distance >= _status.BossStatus.AttackRange[0] && _movementController.Distance <= _status.BossStatus.AttackRange[1])
    //        {
    //            return eNodeState.success;
    //        }

    //        return eNodeState.failure;
    //    }

    //    public eNodeState IsLongRange(DataContext context)
    //    {
    //        Debugger.LogMessage($"원거리 체크일때 _isSkillExecuting : {_isSkillExecuting}");
    //        if (_movementController.Distance > _status.BossStatus.AttackRange[1])
    //        {
    //            return eNodeState.success;
    //        }

    //        return eNodeState.failure;
    //    }

    //    public eNodeState WalkToTarget(DataContext context)
    //    {
    //        if(_movementController.Distance <= _status.BossStatus.AttackRange[1])
    //        {
    //            return eNodeState.success;
    //        }

    //        _movementController.SetLerpRotation();
    //        _movementController.MoveToTarget(_status.BossStatus.ChasingSpeed);
    //        return eNodeState.running;
    //    }
    }
}
