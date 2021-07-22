namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEditor;

    [CustomEditor(typeof(IDataProvider), true)]
    public class IDataProviderEditor : Editor
	{
		private SerializedProperty spTitle;
        private SerializedProperty spDescription;

		private void OnEnable()
		{
            this.spTitle = serializedObject.FindProperty("title");
            this.spDescription = serializedObject.FindProperty("description");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

            EditorGUILayout.LabelField(this.spTitle.stringValue, EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(this.spDescription.stringValue, MessageType.Info);

			serializedObject.ApplyModifiedProperties();
		}
	}
}