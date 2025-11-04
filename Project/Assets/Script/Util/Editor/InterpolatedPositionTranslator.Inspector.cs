#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;
using Backend.Object.Character.Player;

namespace Backend.Util.Editor
{
    [CustomEditor(typeof(InterpolatedPositionTranslator))]
    public class InterpolatedPositionTranslatorInspector : UnityEditor.Editor
    {
        #region CONSTANT FIELD API

        private const string FieldLabel01 = "Speed";
        private const string FieldLabel02 = "Time";

        #endregion

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var component = target as InterpolatedPositionTranslator;
            if (component == null)
            {
                return;
            }

            switch (component.InterpolationMode)
            {
                case InterpolationMode.Lerp:
                    component.Speed = EditorGUILayout.FloatField(FieldLabel01, component.Speed);
                    break;
                case InterpolationMode.SmoothDamp:
                    component.Time =  EditorGUILayout.FloatField(FieldLabel02, component.Time);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(component);
            }
        }
    }
}

#endif
