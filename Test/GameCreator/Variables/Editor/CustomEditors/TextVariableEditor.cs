namespace GameCreator.Variables
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEditor;
	using UnityEditor.UI;
	using GameCreator.Core;

    [CustomEditor(typeof(TextVariable), true)]
	[CanEditMultipleObjects]
    public class TextVariableEditor : UnityEditor.UI.TextEditor
	{
		private SerializedProperty spText;
		private SerializedProperty spFormat;
		private SerializedProperty spVariable;

		protected override void OnEnable()
		{
			base.OnEnable();

            this.spFormat = serializedObject.FindProperty("format");
            this.spVariable = serializedObject.FindProperty("variable");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

            EditorGUILayout.PropertyField(this.spFormat);
            EditorGUILayout.PropertyField(this.spVariable);

            serializedObject.ApplyModifiedProperties();

            serializedObject.Update();
			base.OnInspectorGUI();
            serializedObject.ApplyModifiedProperties();
		}

		// STATIC METHODS: ------------------------------------------------------------------------

        [MenuItem("GameObject/Game Creator/UI/Text (Variable)", false, 20)]
        public static void CreateTextVariable()
		{
            GameObject canvas = CreateSceneObject.GetCanvasGameObject();
            GameObject textGO = DefaultControls.CreateText(CreateSceneObject.GetStandardResources());
            textGO.transform.SetParent(canvas.transform, false);

            DestroyImmediate(textGO.GetComponent<Text>());
            textGO.AddComponent<TextVariable>();
            Selection.activeGameObject = textGO;
		}
	}
}