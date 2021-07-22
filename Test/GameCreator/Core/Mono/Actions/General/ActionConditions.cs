namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
    using GameCreator.Variables;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionConditions : IAction
	{
        public enum Source
        {
            Conditions,
            Variable
        }

        public Source source = Source.Conditions;
        public Conditions conditions;

        [VariableFilter(Variable.DataType.GameObject)]
        public VariableProperty variable = new VariableProperty(Variable.VarType.LocalVariable);

        public bool waitToFinish = true;

		// EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Conditions conditionsToExecute = null;
            switch (this.source)
            {
                case Source.Conditions:
                    conditionsToExecute = this.conditions;
                    break;

                case Source.Variable:
                    GameObject value = this.variable.Get(target) as GameObject;
                    if (value != null) conditionsToExecute = value.GetComponent<Conditions>();
                    break;
            }

            if (!this.waitToFinish)
            {
                if (conditionsToExecute != null) conditionsToExecute.Interact(target);
                return true;
            }

            return false;
        }

        public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
		{
            Conditions conditionsToExecute = null;
            switch (this.source)
            {
                case Source.Conditions:
                    conditionsToExecute = this.conditions;
                    break;

                case Source.Variable:
                    GameObject value = this.variable.Get(target) as GameObject;
                    if (value != null) conditionsToExecute = value.GetComponent<Conditions>();
                    break;
            }

            if (conditionsToExecute != null)
            {
                yield return conditionsToExecute.InteractCoroutine(target);
            }

			yield return 0;
		}

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

        #if UNITY_EDITOR

        public static new string NAME = "General/Call Conditions";
		private const string NODE_TITLE = "Call conditions {0}{1}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spSource;
        private SerializedProperty spConditions;
        private SerializedProperty spVariable;
        private SerializedProperty spWaitToFinish;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
            string conditionsName = (this.source == Source.Conditions
                ? (this.conditions == null ? "none" : this.conditions.name)
                : this.variable.ToString()
            );

            return string.Format(
                NODE_TITLE,
                conditionsName,
                (this.waitToFinish ? " and wait" : "")
            );
		}

		protected override void OnEnableEditorChild ()
		{
            this.spSource = this.serializedObject.FindProperty("source");
            this.spVariable = this.serializedObject.FindProperty("variable");
            this.spConditions = this.serializedObject.FindProperty("conditions");
            this.spWaitToFinish = this.serializedObject.FindProperty("waitToFinish");
		}

		protected override void OnDisableEditorChild ()
		{
            this.spSource = null;
            this.spVariable = null;
            this.spConditions = null;
            this.spWaitToFinish = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spSource);

            EditorGUI.indentLevel++;
            switch (this.spSource.enumValueIndex)
            {
                case (int)Source.Conditions:
                    EditorGUILayout.PropertyField(this.spConditions);
                    break;

                case (int)Source.Variable:
                    EditorGUILayout.PropertyField(this.spVariable);
                    break;
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(this.spWaitToFinish);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
