using GloryDay.BehaviourTree;
using System.Collections;
using System.Collections.Generic;
using Backend.Util.Debug;
using UnityEngine;

namespace Backend.Object.Character.Enemy.Node
{
    public class ConditionAbort : DecoratorNode
    {
        //확장성 키값을 가져온다
        private bool lastConditionValue = false;
        protected override void Start()
        {
            if(blackboard != null)
            {
                lastConditionValue = blackboard.IsParry;
            }
        }
        protected override void Stop()
        {
            if (Child != null)
            {
                Child.Abort();
            }
        }
        protected override State OnUpdate()
        {
            if (blackboard.IsParry != lastConditionValue)
            {
                lastConditionValue = blackboard.IsParry;

                if (blackboard.IsParry == false)
                {
                    if (Child != null && Child.mState == State.Running)
                    {
                        Debugger.LogMessage("하위 노드 중단");
                        Child.Abort();
                    }

                    return State.Failure;
                }
            }

            if(blackboard.IsParry && Child != null)
            {
                return Child.Update();
            }

            return State.Failure;
        }
    }
}
