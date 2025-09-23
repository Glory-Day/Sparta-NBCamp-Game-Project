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
