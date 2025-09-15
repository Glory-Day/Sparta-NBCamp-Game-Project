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


using Ironcow.Synapse;
using Ironcow.Synapse.UI;

using UnityEditor;

using UnityEngine;

[CustomEditor(typeof(CanvasBase), true)]
public class CavnasEditor : SynapseBaseEditor
{
    public override void DrawButton()
    {
        if (GUILayout.Button("Set Parents"))
        {
            var instance = (CanvasOption)target;
            instance.SetParent();
        }
    }
}
