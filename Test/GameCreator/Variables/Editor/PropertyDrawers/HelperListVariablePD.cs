namespace GameCreator.Variables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using GameCreator.Core;

    [CustomPropertyDrawer(typeof(HelperListVariable))]
    public class HelperListVariablePD : PropertyDrawer
    {
        private const float TARGET_WIDTH = 100f;

        // GUI METHODS: ---------------------------------------------------------------------------

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUIContent gcLabel = new GUIContent(label.text);

            SerializedProperty spTargetType = property.FindPropertyRelative("targetType");
            SerializedProperty spTargetObject = property.FindPropertyRelative("targetObject");

            Rect rectTargetType = new Rect(
                position.x,
                position.y,
                EditorGUIUtility.labelWidth + TARGET_WIDTH,
                EditorGUI.GetPropertyHeight(spTargetType, true)
            );
            Rect rectTargetObject = new Rect(
                rectTargetType.x + rectTargetType.width + EditorGUIUtility.standardVerticalSpacing,
                rectTargetType.y,
                position.width - (rectTargetType.width + EditorGUIUtility.standardVerticalSpacing),
                EditorGUI.GetPropertyHeight(spTargetObject, true)
            );

            EditorGUI.PropertyField(rectTargetType, spTargetType, gcLabel);
            EditorGUI.BeginDisabledGroup(spTargetType.intValue != (int)HelperListVariable.Target.GameObject);
            EditorGUI.PropertyField(rectTargetObject, spTargetObject, GUIContent.none);
            EditorGUI.EndDisabledGroup();
        }

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
            SerializedProperty spTargetType = property.FindPropertyRelative("targetType");
            return (
                EditorGUI.GetPropertyHeight(spTargetType, true)
            );
		}
	}
}