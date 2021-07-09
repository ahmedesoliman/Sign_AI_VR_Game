namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
    using UnityEngine.UI;
	using UnityEngine.Events;
	using GameCreator.Core;
    using GameCreator.Variables;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionChangeText : IAction
	{
        public Text text;
        public string content = "{0}";
        public VariableProperty variable = new VariableProperty();

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (this.text != null)
            {
                this.text.text = string.Format(
                    this.content,
                    new string[] { this.variable.ToStringValue(target) }
                );
            }

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

        #if UNITY_EDITOR

        private static readonly GUIContent GUICONTENT_VARIABLE = new GUIContent("{0} Variable");

		public static new string NAME = "UI/Change Text";
        private const string NODE_TITLE = "Change text to {0}";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spText;
        private SerializedProperty spContent;
        private SerializedProperty spVariable;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
            return string.Format(NODE_TITLE, this.content);
		}

		protected override void OnEnableEditorChild ()
		{
            this.spText = this.serializedObject.FindProperty("text");
            this.spContent = this.serializedObject.FindProperty("content");
            this.spVariable = this.serializedObject.FindProperty("variable");
		}

		protected override void OnDisableEditorChild ()
		{
            this.spText = null;
            this.spContent = null;
            this.spVariable = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spText);
            EditorGUILayout.PropertyField(this.spContent);
            EditorGUILayout.PropertyField(this.spVariable, GUICONTENT_VARIABLE);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
