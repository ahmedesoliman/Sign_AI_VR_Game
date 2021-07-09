namespace GameCreator.Variables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using GameCreator.Core;

    [CustomPropertyDrawer(typeof(HelperGetListVariable))]
    public class HelperGetListVariablePD : PropertyDrawer
    {
        private const float TARGET_WIDTH = 100f;

        // GUI METHODS: ---------------------------------------------------------------------------

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUIContent gcLabel = new GUIContent(label.text);
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty spTargetType = property.FindPropertyRelative("targetType");
            SerializedProperty spTargetObject = property.FindPropertyRelative("targetObject");
            SerializedProperty spSelector = property.FindPropertyRelative("select");
            SerializedProperty spIndex = property.FindPropertyRelative("index");

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
            Rect rectSelector = new Rect(
                position.x,
                rectTargetType.y + rectTargetType.height + EditorGUIUtility.standardVerticalSpacing,
                position.width,
                EditorGUIUtility.singleLineHeight
            );

            EditorGUI.PropertyField(rectTargetType, spTargetType, gcLabel);
            EditorGUI.BeginDisabledGroup(spTargetType.intValue != (int)HelperListVariable.Target.GameObject);
            EditorGUI.PropertyField(rectTargetObject, spTargetObject, GUIContent.none);
            EditorGUI.EndDisabledGroup();

            EditorGUI.PropertyField(rectSelector, spSelector);
            if (spSelector.enumValueIndex == (int)ListVariables.Position.Index)
            {
                Rect rectIndex = new Rect(
                    rectSelector.x,
                    rectSelector.y + rectSelector.height + EditorGUIUtility.standardVerticalSpacing,
                    rectSelector.width,
                    EditorGUIUtility.singleLineHeight
                );

                EditorGUI.indentLevel++;
                EditorGUI.PropertyField(rectIndex, spIndex);
                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
            SerializedProperty spTargetType = property.FindPropertyRelative("targetType");
            SerializedProperty spSelector = property.FindPropertyRelative("select");
            SerializedProperty spIndex = property.FindPropertyRelative("index");

            return (
                EditorGUI.GetPropertyHeight(spTargetType, true) +
                EditorGUIUtility.standardVerticalSpacing +
                EditorGUI.GetPropertyHeight(spSelector, true) +
                (spSelector.enumValueIndex == (int)ListVariables.Position.Index
                    ? EditorGUIUtility.standardVerticalSpacing + EditorGUI.GetPropertyHeight(spIndex, true)
                    : 0f
                )
            );
		}
	}
}