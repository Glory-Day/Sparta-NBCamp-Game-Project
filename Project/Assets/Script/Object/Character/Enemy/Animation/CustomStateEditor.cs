using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Backend.Object.Character.Enemy.Animation
{
    [CustomEditor(typeof(StateMachineBase), true)]
    public class CustomStateEditor : Editor
    {
        private SerializedProperty _animationEventsProperty;

        public override void OnInspectorGUI()
        {
            if (_animationEventsProperty == null)
            {
                _animationEventsProperty = serializedObject.FindProperty("AnimationEvents");
            }

            // "AnimationEvents"를 제외한 나머지 필드를 먼저 그립니다.
            DrawPropertiesExcluding(serializedObject, "m_Script", "AnimationEvents");

            serializedObject.Update();

            var stateMachine = (StateMachineBase)target;
            var supportedEventsAttr = stateMachine.GetType().GetCustomAttribute<CustomAttribute>();

            if (supportedEventsAttr != null)
            {
                AnimationEventDrawer.CurrentlySupportedTypes = supportedEventsAttr.EnableEventTypes;
            }
            else
            {
                AnimationEventDrawer.CurrentlySupportedTypes = null;
            }

            if (_animationEventsProperty != null)
            {
                EditorGUILayout.PropertyField(_animationEventsProperty, true);
            }
            else
            {
                EditorGUILayout.HelpBox("AnimationEvents 속성을 찾을 수 없습니다.", MessageType.Warning);
            }

            AnimationEventDrawer.CurrentlySupportedTypes = null;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
