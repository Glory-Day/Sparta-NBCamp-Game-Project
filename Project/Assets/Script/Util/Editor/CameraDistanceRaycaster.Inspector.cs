#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;
using Backend.Object.Character.Player;

namespace Backend.Util.Editor
{
    [CustomEditor(typeof(CameraDistanceRaycaster))]
    public class CameraDistanceRaycasterInspector : UnityEditor.Editor
    {
        #region CONSTANT FIELD API

        private const string FieldLabel01 = "Minimum Casting Distance";
        private const string FieldLabel02 = "Radius";

        #endregion

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var component = target as CameraDistanceRaycaster;
            if (component == null)
            {
                return;
            }

            switch (component.CastMode)
            {
                case CastMode.Raycast:
                    component.MinimumCastingDistance = EditorGUILayout.FloatField(FieldLabel01, component.MinimumCastingDistance);
                    break;
                case CastMode.SphereCast:
                    component.Radius =  EditorGUILayout.FloatField(FieldLabel02, component.Radius);
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
