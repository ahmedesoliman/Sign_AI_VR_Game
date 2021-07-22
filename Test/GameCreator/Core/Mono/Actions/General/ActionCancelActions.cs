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
	public class ActionCancelActions : IAction
	{
        public enum Source
        {
            Actions,
            Variable
        }

        public Source source = Source.Actions;
        public Actions actions;
        [VariableFilter(Variable.DataType.GameObject)]
        public VariableProperty variable = new VariableProperty(Variable.VarType.LocalVariable);

        public bool stopImmidiately = false;

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Actions targetActions = null;
            switch (this.source)
            {
                case Source.Actions:
                    targetActions = this.actions;
                    break;

                case Source.Variable:
                    GameObject value = this.variable.Get(target) as GameObject;
                    if (value != null) targetActions = value.GetComponentInChildren<Actions>();
                    break;
            }

            if (targetActions != null && targetActions.actionsList != null)
            {
                if (this.stopImmidiately) targetActions.Stop();
                else targetActions.actionsList.Cancel();
            }

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

        #if UNITY_EDITOR

        public static new string NAME = "General/Cancel Actions";
        private const string NODE_TITLE = "Cancel action {0} {1}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spStopImmidiately;

        private SerializedProperty spSource;
        private SerializedProperty spActions;
        private SerializedProperty spVariable;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
		{
            string actionsName = string.Empty;
            switch (this.source)
            {
                case Source.Actions:
                    actionsName = (this.actions == null
                        ? "(none)"
                        : this.actions.gameObject.name
                    );
                    break;

                case Source.Variable:
                    actionsName = this.variable.ToString();
                    break;
            }

            return string.Format(
                NODE_TITLE,
                actionsName,
                this.stopImmidiately ? "(immidiately)" : string.Empty
            );
		}

		protected override void OnEnableEditorChild ()
		{
            this.spSource = serializedObject.FindProperty("source");
            this.spActions = serializedObject.FindProperty("actions");
            this.spVariable = serializedObject.FindProperty("variable");
            this.spStopImmidiately = serializedObject.FindProperty("stopImmidiately");
        }

		protected override void OnDisableEditorChild ()
		{
            this.spSource = null;
            this.spActions = null;
            this.spVariable = null;
            this.spStopImmidiately = null;
        }

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spSource);
            switch (this.spSource.enumValueIndex)
            {
                case (int)Source.Actions:
                    EditorGUILayout.PropertyField(this.spActions);
                    break;

                case (int)Source.Variable:
                    EditorGUILayout.PropertyField(this.spVariable);
                    break;
            }
            
            EditorGUILayout.PropertyField(this.spStopImmidiately);
            this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
