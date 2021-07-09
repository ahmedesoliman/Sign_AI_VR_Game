namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using UnityEditor.AnimatedValues;

	[CustomPropertyDrawer (typeof (HPCursor.Data))]
	public class HotspotCursorDrawer : PropertyDrawer
	{
		private const float ICON_SIZE = 65f;
		private const string PROP_ICON = "mouseOverCursor";
		private const string PROP_POSI = "cursorPosition";

		// PAINT METHODS: ----------------------------------------------------------------------------------------------

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) 
		{
			SerializedProperty spIcon = property.FindPropertyRelative(PROP_ICON);
			SerializedProperty spPosi = property.FindPropertyRelative(PROP_POSI);

			EditorGUILayout.BeginHorizontal();

			GUILayoutUtility.GetRect(ICON_SIZE, ICON_SIZE);
			Rect iconRect = new Rect(position.x, position.y, ICON_SIZE, ICON_SIZE);

			spIcon.objectReferenceValue = EditorGUI.ObjectField(
				iconRect,
				spIcon.objectReferenceValue, 
				typeof(Texture2D), 
				false
			);

			Rect centerRect = new Rect(
				position.x + ICON_SIZE + 3f,
				position.y + ICON_SIZE - EditorGUIUtility.singleLineHeight,
				position.width - ICON_SIZE - 3f,
				EditorGUIUtility.singleLineHeight
			);

			EditorGUI.PropertyField(centerRect, spPosi, GUIContent.none);

			EditorGUILayout.EndHorizontal();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) 
		{ 
			return 0f; 
		}
	}
}