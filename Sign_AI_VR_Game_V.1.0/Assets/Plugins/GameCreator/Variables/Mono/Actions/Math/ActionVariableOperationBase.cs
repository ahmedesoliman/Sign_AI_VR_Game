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
	public abstract class ActionVariableOperationBase : IAction
	{
        [VariableFilter(Variable.DataType.Number)]
        public VariableProperty variable = new VariableProperty(Variable.VarType.GlobalVariable);

        public float value = 1f;

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		// PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spVariable;
        private SerializedProperty spValue;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		protected override void OnEnableEditorChild ()
		{
            this.spVariable = this.serializedObject.FindProperty("variable");
            this.spValue = this.serializedObject.FindProperty("value");
		}

		protected override void OnDisableEditorChild ()
		{
            this.spVariable = null;
            this.spValue = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spVariable);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spValue);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
