using System;
using UnityEngine;

namespace Script.Util.Extension
{
    public static class CapsuleColliderExtension
    {
        public static Vector3 GetDirection(this CapsuleCollider capsuleCollider, Transform transform)
        {
            return capsuleCollider.direction switch
            {
                0 => transform.right,
                1 => transform.up,
                2 => transform.forward,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
