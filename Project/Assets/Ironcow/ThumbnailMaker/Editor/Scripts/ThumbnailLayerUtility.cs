// ─────────────────────────────────────────────────────────────────────────────
// Part of the Synapse Framework – © 2025 Ironcow Studio
// This file is distributed under the Unity Asset Store EULA:
// https://unity.com/legal/as-terms
// ─────────────────────────────────────────────────────────────────────────────

using System.Collections.Generic;

using UnityEngine;

namespace Ironcow.Synapse.Thumbnail
{
    public static class ThumbnailLayerUtility
    {
        private static readonly Dictionary<GameObject, int> originalLayers = new();

        /// <summary>
        /// 타겟과 자식 오브젝트들을 썸네일 레이어로 변경하며 기존 레이어 저장
        /// </summary>
        public static void Apply(GameObject target)
        {
            if (target == null) return;

            var layer = ThumbnailMaker.instance.thumbnailLayer;

            foreach (Transform t in target.GetComponentsInChildren<Transform>(true))
            {
                var go = t.gameObject;
                if (!originalLayers.ContainsKey(go))
                    originalLayers[go] = go.layer;

                go.layer = layer;
            }
        }

        /// <summary>
        /// 해당 타겟과 자식 오브젝트들의 레이어를 원래대로 복원
        /// </summary>
        public static void Restore(GameObject target)
        {
            if (target == null) return;

            foreach (Transform t in target.GetComponentsInChildren<Transform>(true))
            {
                var go = t.gameObject;
                if (originalLayers.TryGetValue(go, out int originalLayer))
                {
                    go.layer = originalLayer;
                    originalLayers.Remove(go);
                }
            }
        }

        /// <summary>
        /// 모든 오브젝트 레이어 정보 제거 (안전 장치용)
        /// </summary>
        public static void ClearAll()
        {
            originalLayers.Clear();
        }
    }
}
