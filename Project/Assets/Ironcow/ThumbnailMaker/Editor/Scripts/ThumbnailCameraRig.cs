// ─────────────────────────────────────────────────────────────────────────────
// Part of the Synapse Framework – © 2025 Ironcow Studio
// This file is distributed under the Unity Asset Store EULA:
// https://unity.com/legal/as-terms
// ─────────────────────────────────────────────────────────────────────────────

using UnityEngine;

namespace Ironcow.Synapse.Thumbnail
{
    public class ThumbnailCameraRig
    {
        public Camera CaptureCamera { get; private set; }
        public RenderTexture RenderTexture { get; private set; }

        private GameObject rigRoot;
        private Transform orbitRoot;
        private Transform zoomRoot;

        private Vector2 currentRotation = new Vector2(30f, 0f); // default angle
        private float zoomDistance = 5f;

        private GameObject lastTargetObject;

        public void Create(ThumbnailMaker setting)
        {
            rigRoot = new GameObject("ThumbnailCameraRig");
            rigRoot.AddComponent<ThumbnailRig>();

            orbitRoot = new GameObject("OrbitRoot").transform;
            orbitRoot.SetParent(rigRoot.transform, false);

            zoomRoot = new GameObject("ZoomRoot").transform;
            zoomRoot.SetParent(orbitRoot, false);

            var camGO = new GameObject("ThumbnailCamera");
            CaptureCamera = camGO.AddComponent<Camera>();
            CaptureCamera.transform.SetParent(zoomRoot, false);
            CaptureCamera.clearFlags = CameraClearFlags.SolidColor;
            CaptureCamera.orthographic = false;
            CaptureCamera.allowHDR = false;
            CaptureCamera.allowMSAA = false;
            CaptureCamera.useOcclusionCulling = false;
            CaptureCamera.renderingPath = RenderingPath.Forward;

            zoomDistance = setting.zoom;
            currentRotation = new Vector2(30f, 0f);

            ApplySetting(setting);
        }

        public void SetTarget(Transform target, ThumbnailMaker setting)
        {
            if (target == null) return;

            if (lastTargetObject != null)
            {
                ThumbnailLayerUtility.Restore(lastTargetObject);
            }

            lastTargetObject = target.gameObject;

            Vector3 center = target.position;
            rigRoot.transform.position = center;
            rigRoot.transform.SetParent(target); // keep camera following the target

            orbitRoot.localRotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0f);
            zoomRoot.localPosition = new Vector3(0, 0, -zoomDistance);

            ThumbnailLayerUtility.Apply(lastTargetObject);
        }

        public void Rotate(Vector2 delta)
        {
            currentRotation += delta;
            orbitRoot.localRotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0f);
        }

        public void Zoom(float delta)
        {
            zoomDistance = Mathf.Clamp(zoomDistance + delta, 0.5f, 120f);
            ThumbnailMaker.instance.fieldOfView = zoomDistance;
            CaptureCamera.orthographicSize = zoomDistance;
            CaptureCamera.fieldOfView = zoomDistance;
        }

        public void Pan(Vector2 screenDelta)
        {
            float panSpeed = 0.005f * zoomDistance;

            var right = CaptureCamera.transform.right;
            var up = CaptureCamera.transform.up;

            Vector3 move = (-right * screenDelta.x + up * screenDelta.y) * panSpeed;
            rigRoot.transform.position += move;
        }

        public void Dispose()
        {
            if (lastTargetObject != null)
            {
                ThumbnailLayerUtility.Restore(lastTargetObject);
                lastTargetObject = null;
            }

            if (rigRoot != null)
                Object.DestroyImmediate(rigRoot);
            rigRoot = null;

            if (RenderTexture != null)
                Object.DestroyImmediate(RenderTexture);
            RenderTexture = null;
        }

        public void ApplySetting(ThumbnailMaker setting)
        {
            if (RenderTexture != null)
            {
                if (RenderTexture.width != setting.resolution.x || RenderTexture.height != setting.resolution.y)
                {
                    Object.DestroyImmediate(RenderTexture);
                    AddRenderTexture(setting);
                }
            }
            else
            {
                AddRenderTexture(setting);
            }

            CaptureCamera.fieldOfView = setting.fieldOfView;
            CaptureCamera.orthographicSize = setting.fieldOfView;
            CaptureCamera.targetTexture = RenderTexture;
            CaptureCamera.backgroundColor = setting.backgroundColor;
            CaptureCamera.cullingMask = setting.layerMask;
        }

        public void AddRenderTexture(ThumbnailMaker setting)
        {
            RenderTexture = new RenderTexture(setting.resolution.x, setting.resolution.y, 24, RenderTextureFormat.ARGB32);
            RenderTexture.useMipMap = false;
            RenderTexture.autoGenerateMips = false;
            RenderTexture.Create();
        }
    }
}
