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
	public class ActionVariableRandom : IAction
	{
        public enum Step
        {
            Integer,
            Decimal
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public float minValue = 0.0f;
        public float maxValue = 10.0f;
        public Step step = Step.Decimal;

        [VariableFilter(Variables.Variable.DataType.Number)]
        public VariableProperty variable = new VariableProperty();

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            float random = 0.0f;
            switch (this.step)
            {
                case Step.Decimal : random = Random.Range(this.minValue, this.maxValue); break;
                case Step.Integer : random = (float)Random.Range((int)this.minValue, (int)this.maxValue); break;
            }

            this.variable.Set(random, target);
            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Variables/Variable Random";
        private const string NODE_TITLE = "Random value to {0}";

		// PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spMinValue;
        private SerializedProperty spMaxValue;
        private SerializedProperty spStep;
        private SerializedProperty spVariable;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
            return string.Format(NODE_TITLE, this.variable);
		}

		protected override void OnEnableEditorChild ()
		{
            this.spMinValue = this.serializedObject.FindProperty("minValue");
            this.spMaxValue = this.serializedObject.FindProperty("maxValue");
            this.spStep = this.serializedObject.FindProperty("step");
            this.spVariable = this.serializedObject.FindProperty("variable");
		}

		protected override void OnDisableEditorChild ()
		{
            this.spMinValue = null;
            this.spMaxValue = null;
            this.spStep = null;
            this.spVariable = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spMinValue);
            EditorGUILayout.PropertyField(this.spMaxValue);
            EditorGUILayout.PropertyField(this.spStep);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spVariable);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
