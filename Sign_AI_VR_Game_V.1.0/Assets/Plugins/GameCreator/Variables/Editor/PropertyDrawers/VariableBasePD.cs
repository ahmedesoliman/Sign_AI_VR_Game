namespace GameCreator.Variables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    [CustomPropertyDrawer(typeof(VariableBase), true)]
    public class VariableBasePD : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
            EditorGUI.PropertyField(position, property.FindPropertyRelative("value"), label);
		}
	}
}