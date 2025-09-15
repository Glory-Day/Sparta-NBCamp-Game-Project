// ─────────────────────────────────────────────────────────────────────────────
// Part of the Synapse Framework – © 2025 Ironcow Studio
// This file is distributed under the Unity Asset Store EULA:
// https://unity.com/legal/as-terms
// ─────────────────────────────────────────────────────────────────────────────

using UnityEngine;

namespace Ironcow.Synapse.Thumbnail
{
    public class ThumbnailSession
    {
        public ThumbnailCameraRig CameraRig { get; private set; }
        public Transform Target { get; private set; }
        public string SuggestedName => Target != null ? Target.name : "Thumbnail";

        private readonly ThumbnailMaker setting;
        private GameObject lastTargetObject;

        public ThumbnailSession(ThumbnailMaker setting)
        {
            this.setting = setting;
        }

        public void SetTarget(GameObject obj)
        {
            if (obj == null)
            {
                Target = null;
                return;
            }

            // 🔧 이전 타겟의 레이어 복원
            if (lastTargetObject != null)
            {
                ThumbnailLayerUtility.Restore(lastTargetObject);
            }

            if (CameraRig == null)
            {
                CameraRig = new ThumbnailCameraRig();
                CameraRig.Create(setting);
            }

            Target = obj.transform;
            lastTargetObject = obj;

            CameraRig.SetTarget(Target, setting);
            CameraRig.ApplySetting(setting);
        }

        public void Rotate(Vector2 delta)
        {
            CameraRig?.Rotate(delta);
        }

        public void Pan(Vector2 screenDelta)
        {
            CameraRig?.Pan(screenDelta);
        }

        public void Zoom(float delta)
        {
            CameraRig?.Zoom(delta);
        }

        public void Dispose()
        {
            CameraRig?.Dispose();
            CameraRig = null;
            Target = null;
        }
    }
}
