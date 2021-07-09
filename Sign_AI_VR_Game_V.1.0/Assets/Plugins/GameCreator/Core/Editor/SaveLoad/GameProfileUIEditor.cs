namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEditor;

	[CustomEditor(typeof(GameProfileUI), true)]
	[CanEditMultipleObjects]
    public class GameProfileUIEditor : Editor
	{
		private SerializedProperty spProfile;
        private SerializedProperty spTextProfile;
		private SerializedProperty spFormatProfile;
		private SerializedProperty spTextDate;
        private SerializedProperty spFormatDate;

        private static readonly GUIContent GC_FORMAT = new GUIContent("Format");

		private void OnEnable()
		{
            this.spProfile = serializedObject.FindProperty("profile");
            this.spTextProfile = serializedObject.FindProperty("textProfile");
            this.spFormatProfile = serializedObject.FindProperty("formatProfile");
            this.spTextDate = serializedObject.FindProperty("textDate");
            this.spFormatDate = serializedObject.FindProperty("formatDate");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

            EditorGUILayout.PropertyField(this.spProfile);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(this.spTextProfile);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(this.spFormatProfile, GC_FORMAT);
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(this.spTextDate);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(this.spFormatDate, GC_FORMAT);
            EditorGUI.indentLevel--;

			serializedObject.ApplyModifiedProperties();
		}

		// STATIC METHODS: ------------------------------------------------------------------------

		[MenuItem("GameObject/Game Creator/UI/Game Profile", false, 100)]
		public static void CreateGameProfileUI()
		{
			GameObject text = CreateSceneObject.Create("Profile");
            text.AddComponent<GameProfileUI>();

            text.transform.localRotation = Quaternion.identity;
            text.transform.localScale = Vector3.one;
		}
	}
}