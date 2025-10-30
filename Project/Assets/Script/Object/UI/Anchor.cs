using UnityEngine;

namespace Backend.Object.UI
{
    public static class Anchor
    {
        public static Vector2 LeftTop { get; } = new (0f, 1f);
        public static Vector2 LeftBottom { get; } = new (0f, 0f);
        public static Vector2 RightTop { get; } = new (1f, 1f);
        public static Vector2 RightBottom { get; } = new (1f, 0f);
        public static Vector2 Center { get; } = new (0.5f, 0.5f);
    }
}
