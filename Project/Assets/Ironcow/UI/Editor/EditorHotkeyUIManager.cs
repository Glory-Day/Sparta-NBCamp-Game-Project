// ─────────────────────────────────────────────────────────────────────────────
// Part of the Synapse Framework © 2025 Ironcow Studio
// Distributed via Gumroad under a paid license
// 
// 🔐 This file is part of a licensed product. Redistribution or sharing is prohibited.
// 🔑 A valid license key is required to unlock all features.
// 
// 🌐 For license terms, support, or team licensing, visit:
//     https://ironcowstudio.duckdns.org/ironcowstudio.html
// ─────────────────────────────────────────────────────────────────────────────


using System.IO;

using UnityEditor;

using UnityEngine;

namespace Ironcow.Synapse.UI
{
    public partial class EditorHotkeyUIManager : EditorSingleton<EditorHotkeyUIManager>
    {
        [SerializeField] public DefaultAsset uiTemplateFolder;
        public DefaultAsset UITemplateFolder
        {
            get
            {
                if (uiTemplateFolder == null)
                {
                    // 현재 클래스가 정의된 스크립트 경로 가져오기
                    var script = MonoScript.FromScriptableObject(this);
                    string scriptPath = AssetDatabase.GetAssetPath(script);

                    // 상위 폴더 기준으로 Template 경로 계산
                    string editorDir = Path.GetDirectoryName(scriptPath);
                    string parentDir = Path.GetDirectoryName(editorDir);
                    string templatePath = Path.Combine(parentDir, "Template");

                    // 경로 정리
                    templatePath = templatePath.Replace("\\", "/");

                    // 폴더를 DefaultAsset으로 로드
                    uiTemplateFolder = AssetDatabase.LoadAssetAtPath<DefaultAsset>(templatePath);

                    var meta = GetMetaFile();
                    meta = meta.Replace("uiTemplateFolder: {instanceID: 0}", "uiTemplateFolder: {instanceID: " + uiTemplateFolder.GetInstanceID() + "}");
                    SaveMetaFile(meta);
                    AssetDatabase.Refresh();
                }
                return uiTemplateFolder;
            }
        }

        public string GetMetaFile()
        {
            var script = MonoScript.FromScriptableObject(this);
            string scriptPath = AssetDatabase.GetAssetPath(script);
            scriptPath += ".meta";
            scriptPath = scriptPath.Replace("Assets", Application.dataPath);
            if (File.Exists(scriptPath))
            {
                return File.ReadAllText(scriptPath);
            }
            return "";
        }

        public void SaveMetaFile(string text)
        {
            var script = MonoScript.FromScriptableObject(this);
            string scriptPath = AssetDatabase.GetAssetPath(script);
            scriptPath += ".meta";
            scriptPath = scriptPath.Replace("Assets", Application.dataPath);
            if (File.Exists(scriptPath))
            {
                File.WriteAllText(scriptPath, text);
            }
        }

        static string GetUITempletePath(string filename)
        {
            var path = Path.Combine(AssetDatabase.GetAssetPath(instance.UITemplateFolder), filename).Replace("Assets", Application.dataPath).Replace("\\", "/");
            return path;
        }

        [MenuItem("Assets/Create/SynapseScript/UI/UIBase Script", false, -539)]
        static void CreateUIBaseScript()
        {
            var template = GetUITempletePath("UIBaseTemplate.cs.txt");
            var dest = "UI.cs";
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(template, dest);
        }

        [MenuItem("Assets/Create/SynapseScript/UI/UIListBase Script", false, -538)]
        static void CreateUIListBaseScript()
        {
            var template = GetUITempletePath("UIListBaseTemplate.cs.txt");
            var dest = "UI.cs";
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(template, dest);
        }

        [MenuItem("Assets/Create/SynapseScript/UI/UIPopupScript", false, -537)]
        static void CreateUIPopupScript()
        {
            var template = GetUITempletePath("UIPopupTemplate.cs.txt");
            var dest = "Popup.cs";
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(template, dest);
        }

        [MenuItem("Assets/Create/SynapseScript/UI/UICanvasScript", false, -536)]
        static void CreateUICanvasScript()
        {
            var template = GetUITempletePath("CanvasBaseTemplate.cs.txt");
            var dest = "Canvas.cs";
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(template, dest);
        }

        [MenuItem("Assets/Create/SynapseScript/UI/UIListItemScript", false, -535)]
        static void CreateUIListItemScript()
        {
            var template = GetUITempletePath("UIListItemTemplate.cs.txt");
            var dest = "Item.cs";
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(template, dest);
        }

    }
}
