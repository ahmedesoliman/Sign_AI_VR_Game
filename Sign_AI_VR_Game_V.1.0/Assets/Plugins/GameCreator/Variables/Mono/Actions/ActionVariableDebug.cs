namespace GameCreator.Variables
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using GameCreator.Core;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionVariableDebug : IAction
	{
        private const string LOG_FMT = "{0}: {1}";

        // PROPERTIES: ----------------------------------------------------------------------------

        public VariableProperty variable = new VariableProperty();

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Debug.LogFormat(
                LOG_FMT,
                this.variable.ToString(),
                this.variable.ToStringValue(target)
            );

            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Debug/Debug Variable";
        private const string NODE_TITLE = "Log variable {0}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spVariable;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
            return string.Format(NODE_TITLE, this.variable.ToString());
		}

		protected override void OnEnableEditorChild ()
		{
            this.spVariable = this.serializedObject.FindProperty("variable");
		}

		protected override void OnDisableEditorChild ()
		{
            this.spVariable = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();
            EditorGUILayout.PropertyField(this.spVariable);
			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
