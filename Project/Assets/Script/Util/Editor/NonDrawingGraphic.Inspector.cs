using System.Collections;
using System.Collections.Generic;
using Backend.Object.UI;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace Backend.Util.Editor
{
    [CanEditMultipleObjects, CustomEditor(typeof(NonDrawingGraphic))]
    public class NonDrawingGraphicInspector : GraphicEditor
    {
        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            EditorGUILayout.PropertyField(base.m_Script, new GUILayoutOption[0]);

            base.RaycastControlsGUI();
            base.serializedObject.ApplyModifiedProperties();
        }
    }
}

