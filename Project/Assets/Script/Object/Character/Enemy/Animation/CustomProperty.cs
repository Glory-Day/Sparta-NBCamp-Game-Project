using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Backend.Object.Character.Enemy.Animation
{
    [CustomPropertyDrawer(typeof(AnimationEvent))]
    public class AnimationEventDrawer : PropertyDrawer
    {
        public static AnimationEvent.EventType[] CurrentlySupportedTypes;

        private const float VerticalSpacing = 1.2f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property.isExpanded, label);

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                var currentPos = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);

                var typeEventProp = property.FindPropertyRelative("TypeEvent");
                var descriptProp = property.FindPropertyRelative("Descript");
                var normalizeTimeProp = property.FindPropertyRelative("NormalizeTime");
                var indexProp = property.FindPropertyRelative("Index");
                var valueProp = property.FindPropertyRelative("Value");
                var booleanProp = property.FindPropertyRelative("IsBool");

                //필터링된 Popup
                if (CurrentlySupportedTypes != null && CurrentlySupportedTypes.Length > 0)
                {
                    var supportedNames = CurrentlySupportedTypes.Select(t => t.ToString()).ToArray();
                    var supportedValues = CurrentlySupportedTypes.Select(t => (int)t).ToArray();

                    int currentEnumValue = typeEventProp.enumValueIndex;
                    int currentIndexInSupported = Array.IndexOf(supportedValues, currentEnumValue);

                    if (currentIndexInSupported < 0)
                    {
                        currentIndexInSupported = 0;
                        typeEventProp.enumValueIndex = supportedValues[0];
                    }

                    int newIndex = EditorGUI.Popup(currentPos, "Type Event", currentIndexInSupported, supportedNames);
                    if (newIndex != currentIndexInSupported)
                    {
                        typeEventProp.enumValueIndex = supportedValues[newIndex];
                    }
                }
                else //지원 목록이 없다면 기본 Enum 필드
                {
                    EditorGUI.PropertyField(currentPos, typeEventProp);
                }

                // 나머지 필드.
                currentPos.y += EditorGUIUtility.singleLineHeight * VerticalSpacing;
                EditorGUI.PropertyField(currentPos, descriptProp);
                currentPos.y += EditorGUIUtility.singleLineHeight * VerticalSpacing;
                EditorGUI.PropertyField(currentPos, normalizeTimeProp);
                currentPos.y += EditorGUIUtility.singleLineHeight * VerticalSpacing;

                // 선택된 EventType에 따라 필요한 필드만 보여주기.
                var currentSelectedType = (AnimationEvent.EventType)typeEventProp.enumValueIndex;
                switch (currentSelectedType)
                {
                    case AnimationEvent.EventType.SetEffect:
                    case AnimationEvent.EventType.SetWeapon:
                        EditorGUI.PropertyField(currentPos, indexProp);
                        currentPos.y += EditorGUIUtility.singleLineHeight * VerticalSpacing;
                        break;
                    case AnimationEvent.EventType.SetSpeed:
                        EditorGUI.PropertyField(currentPos, valueProp);
                        currentPos.y += EditorGUIUtility.singleLineHeight * VerticalSpacing;
                        break;
                    case AnimationEvent.EventType.SetParry:
                        EditorGUI.PropertyField(currentPos, booleanProp);
                        currentPos.y += EditorGUIUtility.singleLineHeight * VerticalSpacing;
                        break;
                    case AnimationEvent.EventType.PlayEffect:
                    case AnimationEvent.EventType.StopEffect:
                    case AnimationEvent.EventType.StartAttack:
                    case AnimationEvent.EventType.EndAttack:
                        currentPos.y += EditorGUIUtility.singleLineHeight * VerticalSpacing;
                        break;
                }

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        // 그려야 할 UI의 전체 높이를 계산하여 반환.
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float totalHeight = EditorGUIUtility.singleLineHeight * VerticalSpacing;

            if (property.isExpanded)
            {
                totalHeight += EditorGUIUtility.singleLineHeight * 3 * VerticalSpacing; // 기본 3개 필드 (Type, Descript, NormalizeTime) 높이

                var typeEventProp = property.FindPropertyRelative("TypeEvent");
                var currentSelectedType = (AnimationEvent.EventType)typeEventProp.enumValueIndex;

                // 필드의 높이를 추가합니다.
                switch (currentSelectedType)
                {
                    case AnimationEvent.EventType.SetEffect:
                    case AnimationEvent.EventType.SetWeapon:
                    case AnimationEvent.EventType.SetSpeed:
                    case AnimationEvent.EventType.SetParry:
                        totalHeight += EditorGUIUtility.singleLineHeight * VerticalSpacing;
                        break;
                }
            }
            return totalHeight;
        }
    }
}
