namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;

	[CustomPropertyDrawer (typeof (HPProximity.Data))]
	public class HotspotProximityDrawer : PropertyDrawer
	{
		private const string PROP_PREFAB = "prefab";
        private const string PROP_OFFSET = "offset";
        private const string PROP_RADIUS = "radius";
		private const string PROP_TARGET = "target";
		private const string PROP_TARGET_PLAYER = "targetPlayer";

		// PAINT METHODS: ----------------------------------------------------------------------------------------------

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) 
		{
			SerializedProperty spPrefab = property.FindPropertyRelative(PROP_PREFAB);
            SerializedProperty spOffset = property.FindPropertyRelative(PROP_OFFSET);
            SerializedProperty spRadius = property.FindPropertyRelative(PROP_RADIUS);
			SerializedProperty spTarget = property.FindPropertyRelative(PROP_TARGET);
			SerializedProperty spTargetPlayer = property.FindPropertyRelative(PROP_TARGET_PLAYER);

			spPrefab.objectReferenceValue = EditorGUILayout.ObjectField(
				spPrefab.displayName,
				spPrefab.objectReferenceValue, 
				typeof(GameObject),
				false
			);

            EditorGUILayout.PropertyField(spOffset);
			EditorGUILayout.PropertyField(spRadius);

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(spTargetPlayer);

			EditorGUI.BeginDisabledGroup(spTargetPlayer.boolValue);
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(spTarget);
			EditorGUI.indentLevel--;
			EditorGUI.EndDisabledGroup();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) 
		{ 
			return 0f; 
		}
	}
}