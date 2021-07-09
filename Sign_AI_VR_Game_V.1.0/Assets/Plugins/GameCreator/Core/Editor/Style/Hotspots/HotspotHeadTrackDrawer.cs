namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;

	[CustomPropertyDrawer (typeof (HPHeadTrack.Data))]
	public class HotspotHeadTrackDrawer : PropertyDrawer
	{
		private const string PROP_CHARACTERS = "characters";
		private const string PROP_RADIUS = "radius";

		// PAINT METHODS: ----------------------------------------------------------------------------------------------

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) 
		{
			SerializedProperty spCharacters = property.FindPropertyRelative(PROP_CHARACTERS);
			SerializedProperty spRadius = property.FindPropertyRelative(PROP_RADIUS);

			EditorGUILayout.PropertyField(spRadius);
			EditorGUILayout.Space();
			this.PaintCharactersList(spCharacters);
		}

		private void PaintCharactersList(SerializedProperty characters)
		{
			EditorGUILayout.BeginVertical(CoreGUIStyles.GetHelpBox());

			int removeIndex = -1;
			int numCharacters = characters.arraySize;
			for (int i = 0; i < numCharacters; ++i)
			{
				SerializedProperty element = characters.GetArrayElementAtIndex(i);
                GUIContent targetContent = new GUIContent(string.Format("Character {0}", i));

				EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                EditorGUILayout.PropertyField(element, targetContent);
				if (GUILayout.Button("-", EditorStyles.miniButton, GUILayout.Width(25f)))
				{
					removeIndex = i;
				}

				EditorGUILayout.EndHorizontal();
			}

			if (GUILayout.Button("+"))
			{
				characters.InsertArrayElementAtIndex(numCharacters);
			}

			if (removeIndex != -1)
			{
				characters.DeleteArrayElementAtIndex(removeIndex);
			}

			EditorGUILayout.EndVertical();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) 
		{ 
			return 0f; 
		}
	}
}