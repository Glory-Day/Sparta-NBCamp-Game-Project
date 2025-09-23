using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Backend.Object.Character.Enemy.Node
{
    public class CoolDownCheck : ActionNode
    {
        protected override State OnUpdate()
        {
            if (agent.CombatController.ActionData != null)
            {
                if (agent.CombatController.ActionCoolTimer[agent.CombatController.ActionData.ID].IsCoolingDown)
                {
                    return State.Failure;
                }
                else
                {
                    return State.Success;
                }
            }

            return State.Failure;
        }
        protected override void Start()
        {

        }
        protected override void Stop()
        {

        }
    }
}

//        // 쿨다운 확인 후 사용 가능한 스킬 리스트에 추가
//        public eNodeState CoolDownCheck(DataContext context) //특수 스킬, 노말 스킬, 10, 20, 0, 0
//        {
//            if (_isSkillExecuting)
//            {

//                return eNodeState.failure;
//            }

//            foreach (string cool in _cooltimer.Keys)
//            {
//                if (!_cooltimer[cool].IsCoolingDown)
//                {
//                    _usableSkills.Add(_skillCache[cool]);
//                }
//            }
//            if (_usableSkills.Count <= 0)
//            {
//                return eNodeState.failure;
//            }
//            else
//            {
//                return eNodeState.success;
//            }
//        }
