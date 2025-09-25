using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Backend.Object.Character.Enemy.Node
{
    public class CoolDownCheckTest : ActionNode
    {
        public string SkillID;
        protected override State OnUpdate()
        {
            if (agent.CombatController.ActionData != null)
            {
                if (agent.CombatController.ActionCoolTimer[SkillID].IsCoolingDown)
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
