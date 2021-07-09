namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using GameCreator.Core;
    using GameCreator.Variables;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
    public abstract class IActionVariablesAssign : IAction
	{
        public enum ValueFrom
        {
            Player,
            Invoker,
            Constant,
            GlobalVariable,
            LocalVariable,
            ListVariable
        }

        public ValueFrom valueFrom = ValueFrom.Constant;
        public HelperGlobalVariable global = new HelperGlobalVariable();
        public HelperLocalVariable local = new HelperLocalVariable();
        public HelperGetListVariable list = new HelperGetListVariable();

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            this.ExecuteAssignement(target);
            return true;
        }

        public abstract void ExecuteAssignement(GameObject target);

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Variables/Variable Assign";
        protected const string NODE_TITLE = "Assign {0} to {1}";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spVariable;
        private SerializedProperty spValueFrom;
        private SerializedProperty spValue;
        private SerializedProperty spGlobal;
        private SerializedProperty spLocal; 
        private SerializedProperty spList;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
		{
            return string.Format(NODE_TITLE, "(none)", "(none)");
		}

		protected override void OnEnableEditorChild ()
		{
            this.spVariable = this.serializedObject.FindProperty("variable");
            this.spValueFrom = this.serializedObject.FindProperty("valueFrom");
            this.spValue = this.serializedObject.FindProperty("value");
            this.spGlobal = this.serializedObject.FindProperty("global");
            this.spLocal = this.serializedObject.FindProperty("local");
            this.spList = this.serializedObject.FindProperty("list");
        }

		protected override void OnDisableEditorChild ()
		{
            this.spVariable = null;
            this.spValueFrom = null;
            this.spValue = null;
            this.spGlobal = null;
            this.spLocal = null;
            this.spList = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spVariable);
            EditorGUILayout.Space();

            if (this.PaintInspectorTarget())
            {
                EditorGUILayout.PropertyField(this.spValueFrom);
                switch ((ValueFrom)this.spValueFrom.enumValueIndex)
                {
                    case ValueFrom.Constant:
                        EditorGUILayout.PropertyField(this.spValue);
                        break;

                    case ValueFrom.GlobalVariable:
                        EditorGUILayout.PropertyField(this.spGlobal);
                        break;

                    case ValueFrom.LocalVariable:
                        EditorGUILayout.PropertyField(this.spLocal);
                        break;

                    case ValueFrom.ListVariable:
                        EditorGUILayout.PropertyField(this.spList);
                        break;
                }
            }
            else
            {
                EditorGUILayout.PropertyField(this.spValue);
            }

			this.serializedObject.ApplyModifiedProperties();
		}

        public abstract bool PaintInspectorTarget();

		#endif
	}
}
