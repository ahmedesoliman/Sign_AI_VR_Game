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
	public class ActionVariableMath : IAction
	{
        private const string HELPBOX = "Evaluates a math expression and saves it in variable Result";

        // PROPERTIES: ----------------------------------------------------------------------------

        public string expression = "{0} + 1";

        [VariableFilter(Variable.DataType.Number)]
        public VariableProperty result = new VariableProperty();

        [VariableFilter(Variable.DataType.Number)]
        public VariableProperty variable1 = new VariableProperty();

        [VariableFilter(Variable.DataType.Number)]
        public VariableProperty variable2 = new VariableProperty();

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            string mathExpression = string.Format(
                this.expression,
                (float)(this.result.Get(target) ?? 0f),
                (float)(this.variable1.Get(target) ?? 0f),
                (float)(this.variable2.Get(target) ?? 0f)
            );

            float value = GameCreator.Core.ExpressionEvaluator.Evaluate(mathExpression);
            this.result.Set(value, target);

            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Variables/Variable Math";
        private const string NODE_TITLE = "Math expression {0}";

        private static readonly GUIContent GUICONTENT_EXP = new GUIContent("Expression");
        private static readonly GUIContent GUICONTENT_RES = new GUIContent("{0} Result");
        private static readonly GUIContent GUICONTENT_VR1 = new GUIContent("{1} Variable");
        private static readonly GUIContent GUICONTENT_VR2 = new GUIContent("{2} Variable");

		// PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spExpression;
        private SerializedProperty spResult;
        private SerializedProperty spVariable1;
        private SerializedProperty spVariable2;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
            return string.Format(NODE_TITLE, this.result);
		}

		protected override void OnEnableEditorChild ()
		{
            this.spExpression = this.serializedObject.FindProperty("expression");
            this.spResult = this.serializedObject.FindProperty("result");
            this.spVariable1 = this.serializedObject.FindProperty("variable1");
            this.spVariable2 = this.serializedObject.FindProperty("variable2");
		}

		protected override void OnDisableEditorChild ()
		{
            this.spExpression = null;
            this.spResult = null;
            this.spVariable1 = null;
            this.spVariable2 = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.HelpBox(HELPBOX, MessageType.Info);

            EditorGUILayout.PropertyField(this.spExpression, GUICONTENT_EXP);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spResult, GUICONTENT_RES);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spVariable1, GUICONTENT_VR1);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spVariable2, GUICONTENT_VR2);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
