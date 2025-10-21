using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Backend.Object.Character.Enemy.Animation
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class CustomAttribute : Attribute
    {
        public AnimationEvent.EventType[] EnableEventTypes { get; }

        public CustomAttribute(params AnimationEvent.EventType[] eventTypes)
        {
            EnableEventTypes = eventTypes;
        }
    }
}
