namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;

	[CustomPropertyDrawer (typeof (RotationConstraintAttribute))]
	public class RotationConstraintDrawer : PropertyDrawer 
	{
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) 
		{
			EditorGUI.BeginProperty (position, label, property);

			position = EditorGUI.PrefixLabel (position, GUIUtility.GetControlID (FocusType.Passive), label);
			var indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			float elementWidth = position.width/3.0f;

			Rect constraintXRect = new Rect (position.x + (elementWidth * 0), position.y, elementWidth, position.height);
			Rect constraintYRect = new Rect (position.x + (elementWidth * 1), position.y, elementWidth, position.height);
			Rect constraintZRect = new Rect (position.x + (elementWidth * 2), position.y, elementWidth, position.height);

			bool constraintX = (property.vector3Value.x == 0 ? false : true);
			bool constraintY = (property.vector3Value.y == 0 ? false : true);
			bool constraintZ = (property.vector3Value.z == 0 ? false : true);

			constraintX = EditorGUI.ToggleLeft(constraintXRect, "X", constraintX);
			constraintY = EditorGUI.ToggleLeft(constraintYRect, "Y", constraintY);
			constraintZ = EditorGUI.ToggleLeft(constraintZRect, "Z", constraintZ);

			EditorGUI.indentLevel = indent;

			property.vector3Value = new Vector3(
				(constraintX ? 1 : 0),
				(constraintY ? 1 : 0),
				(constraintZ ? 1 : 0)
			);

			EditorGUI.EndProperty();
		}
	}
}