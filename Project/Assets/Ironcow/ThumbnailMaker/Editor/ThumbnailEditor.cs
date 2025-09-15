// ─────────────────────────────────────────────────────────────────────────────
// Part of the Synapse Framework – © 2025 Ironcow Studio
// This file is distributed under the Unity Asset Store EULA:
// https://unity.com/legal/as-terms
// ─────────────────────────────────────────────────────────────────────────────

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;

using UnityEditor;

using UnityEngine;

namespace Ironcow.Synapse.Thumbnail
{
    [CustomEditor(typeof(ThumbnailMaker))]
    public class ThumbnailSettingEditor : Editor
    {
        private ThumbnailSession session;
        private GameObject lastSelected;
        private string customName = "";

        private Rect previewRect;
        private Vector2 prevMousePos;
        private bool isDragging = false;
        private int dragButton = -1;

        private bool isLock;
        private bool isOrtho;

        private bool isChainShot;
        private float chainShotTime;
        private float chainShotTiming;

        private void OnEnable()
        {
            // 씬/에디터 상에 떠 있는 임시 ThumbnailRig 정리
            var leftovers = GameObject.FindObjectsByType<ThumbnailRig>(FindObjectsSortMode.InstanceID);
             
            foreach (var obj in leftovers)
            {
                UnityEngine.Object.DestroyImmediate(obj.gameObject);
            }

            // 다시 정상 상태로 초기화
            ThumbnailMaker.ReleaseCallback += Destroy;
        }



        public void Destroy()
        {
            if (session != null)
                session.Dispose();
            ThumbnailMaker.ReleaseCallback -= Destroy;
        }

        public override void OnInspectorGUI()
        {
            OnDraw();
        }

        public void OnDraw()
        {
            var setting = (ThumbnailMaker)target;
            serializedObject.Update();

            UpdateSelectionWatcher();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("resolution"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("backgroundColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("zoom"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("saveFolder"));
            EditorGUILayout.Slider(serializedObject.FindProperty("fieldOfView"), 1f, 120f);
            SerializedProperty layerProp = serializedObject.FindProperty("layerMask");

            int newMask = EditorGUILayout.MaskField(
                "Layer Mask",
                LayerMaskToField(layerProp.intValue),
                UnityEditorInternal.InternalEditorUtility.layers
            );

            int convertedMask = FieldToLayerMask(newMask);
            if (layerProp.intValue != convertedMask)
            {
                layerProp.intValue = convertedMask;
                serializedObject.ApplyModifiedProperties();
                session?.CameraRig?.ApplySetting((ThumbnailMaker)target);
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();

                // 여기가 핵심
                session?.CameraRig?.ApplySetting((ThumbnailMaker)target);
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                HandleUtility.Repaint(); // 뷰 강제 갱신
            }

            GUILayout.Space(5);
            if (session == null || session.CameraRig?.RenderTexture == null)
            {
                EditorGUILayout.HelpBox("Select a GameObject in the hierarchy to preview.", MessageType.Info);
                return;
            }

            DrawCameraRect(setting);
        }

        private void DrawCameraRect(ThumbnailMaker setting)
        {
            if (session == null || session.CameraRig == null || session.CameraRig.RenderTexture == null) return;
            GUI.changed = true; // IMGUI 내부 상태를 더티하게 만듦
            previewRect = GUILayoutUtility.GetAspectRect(1f);// OnGUI Repaint 시
            EditorGUI.DrawPreviewTexture(previewRect, session.CameraRig?.RenderTexture);

            HandleMouseEvents(setting);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace(); // 🔥 이게 핵심! 좌측 공간 밀어줌
                isOrtho = GUILayout.Toggle(isOrtho, !isOrtho ? "Perspective" : "Orthographic", GUI.skin.button, GUILayout.Width(90));
                isLock = GUILayout.Toggle(isLock, isLock ? "🔒 Lock" : "🔓 Lock", GUI.skin.button, GUILayout.Width(70));
                if (session?.CameraRig?.CaptureCamera is Camera cam)
                {
                    cam.orthographic = isOrtho;
                }
            }

            GUILayout.Space(5);
            customName = EditorGUILayout.TextField("File Name", customName);
            isChainShot = GUILayout.Toggle(isChainShot, "ChainShot");
            if (isChainShot)
            {
                chainShotTime = EditorGUILayout.FloatField("Chain Shot Time", chainShotTime);
                chainShotTiming = EditorGUILayout.FloatField("Chain Shot Timing", chainShotTiming);
            }
            if (GUILayout.Button("Capture Thumbnail", GUILayout.Height(25)))
            {
                if (isChainShot) CaptureChainShot();
                else SaveThumbnail(setting, customName);
            }
        }

        private int LayerMaskToField(int mask)
        {
            int result = 0;
            string[] layers = UnityEditorInternal.InternalEditorUtility.layers;
            for (int i = 0; i < layers.Length; i++)
            {
                int bit = 1 << LayerMask.NameToLayer(layers[i]);
                if ((mask & bit) != 0)
                    result |= 1 << i;
            }
            return result;
        }

        private int FieldToLayerMask(int field)
        {
            int result = 0;
            string[] layers = UnityEditorInternal.InternalEditorUtility.layers;
            for (int i = 0; i < layers.Length; i++)
            {
                if ((field & (1 << i)) != 0)
                    result |= 1 << LayerMask.NameToLayer(layers[i]);
            }
            return result;
        }

        private void UpdateSelectionWatcher()
        {
            if (!isLock && Selection.activeGameObject != null && Selection.activeGameObject != lastSelected)
            {
                lastSelected = Selection.activeGameObject;
                var setting = (ThumbnailMaker)target;

                if (session == null)
                    session = new ThumbnailSession(setting);

                session.SetTarget(lastSelected);
                customName = session.SuggestedName;
            }
        }

        private void HandleMouseEvents(ThumbnailMaker setting)
        {
            var e = Event.current;

            if (e.type == EventType.MouseDown && previewRect.Contains(e.mousePosition))
            {
                isDragging = true;
                dragButton = e.button;
                prevMousePos = e.mousePosition;
                e.Use();
            }
            else if (e.type == EventType.MouseDrag && isDragging)
            {
                Vector2 delta = e.mousePosition - prevMousePos;
                prevMousePos = e.mousePosition;

                if (dragButton == 0)
                    session.Rotate(new Vector2(delta.y, delta.x));
                else if (dragButton == 2)
                    session.Pan(delta);

                //HandleUtility.Repaint();
                e.Use();
            }
            else if (e.type == EventType.MouseUp)
            {
                isDragging = false;
                dragButton = -1;
            }

            if (e.type == EventType.ScrollWheel && previewRect.Contains(e.mousePosition))
            {
                float zoomDelta = e.delta.y * 0.05f;
                session.Zoom(zoomDelta);
                //HandleUtility.Repaint();
                e.Use();
            }

            EditorGUIUtility.AddCursorRect(previewRect, MouseCursor.Pan);
        }

        private async void CaptureChainShot()
        {
            var setting = (ThumbnailMaker)target;
            var t = 0f;
            int cnt = 0;
            List<Texture2D> textures = new List<Texture2D>();
            while (t < chainShotTime)
            {
                cnt++;
                await Task.Delay((int)(chainShotTiming * 1000));
                t += chainShotTiming;
                //SaveThumbnail(setting, customName + "_" + cnt);
                textures.Add(GetTexture());
            }
            for (int i = 0; i < textures.Count; i++)
            {
                SaveThumbnail(textures[i], setting, customName + "_" + (i + 1));
            }
        }

        private Texture2D GetTexture()
        {
            var rt = session.CameraRig.RenderTexture;
            var currentRT = RenderTexture.active;
            RenderTexture.active = rt;

            Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
            tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            tex.Apply();

            RenderTexture.active = currentRT;
            return tex;
        }

        private void SaveThumbnail(Texture2D tex, ThumbnailMaker setting, string filename)
        {
            string folder = setting.GetSavePath();
            if (string.IsNullOrEmpty(folder))
            {
                Debug.LogError("Save path is invalid.");
                return;
            }

            string path = System.IO.Path.Combine(folder, $"{filename}.png");
            System.IO.File.WriteAllBytes(path, tex.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(tex);
            AssetDatabase.ImportAsset(path);

            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.alphaIsTransparency = true;
                importer.mipmapEnabled = false;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.filterMode = FilterMode.Bilinear;
                EditorUtility.SetDirty(importer);
                importer.SaveAndReimport();
            }

            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Texture2D>(path));
        }
        private void SaveThumbnail(ThumbnailMaker setting, string filename)
        {
            var tex = GetTexture();

            string folder = setting.GetSavePath();
            if (string.IsNullOrEmpty(folder))
            {
                Debug.LogError("Save path is invalid.");
                return;
            }

            string path = System.IO.Path.Combine(folder, $"{filename}.png");
            System.IO.File.WriteAllBytes(path, tex.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(tex);
            AssetDatabase.ImportAsset(path);

            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.alphaIsTransparency = true;
                importer.mipmapEnabled = false;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.filterMode = FilterMode.Bilinear;
                EditorUtility.SetDirty(importer);
                importer.SaveAndReimport();
            }

            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Texture2D>(path));
        }
    }

}
