// ─────────────────────────────────────────────────────────────────────────────
// Part of the Synapse Framework – © 2025 Ironcow Studio
// This file is distributed under the Unity Asset Store EULA:
// https://unity.com/legal/as-terms
// ─────────────────────────────────────────────────────────────────────────────

using UnityEngine;

using UnityEditor;

namespace Ironcow.Synapse.Thumbnail
{
    [CreateAssetMenu(menuName = "Synapse/Thumbnail/ThumbnailSetting")]
    public class ThumbnailMaker : SOSingleton<ThumbnailMaker>
    {
        public LayerMask layerMask = ~0; // 기본: Everything

        [Header("Resolution (in pixels)")]
        public Vector2Int resolution = new(512, 512);

        [Header("Background Color")]
        public Color backgroundColor = new(0f, 0f, 0f, 0f); // 투명

        [Header("Camera Settings")]
        public float zoom = 5f;

        [Header("Save Folder")]
        public DefaultAsset saveFolder;

        [Header("Layer Override")]
        public int thumbnailLayer = 31; // default: 맨 마지막 사용자 레이어

        [Range(1f, 120f)]
        public float fieldOfView = 30f; // 좁을수록 확대 효과

        public string GetSavePath()
        {
            return saveFolder != null
                ? AssetDatabase.GetAssetPath(saveFolder)
                : "Assets/Thumbnails";
        }
    }
}
