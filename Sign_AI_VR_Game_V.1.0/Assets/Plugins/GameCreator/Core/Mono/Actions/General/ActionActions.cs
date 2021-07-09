namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
    using GameCreator.Variables;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionActions : IAction 
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

		public bool waitToFinish = false;

        private bool actionsComplete = false;
        private bool forceStop = false;

		// EXECUTABLE: ----------------------------------------------------------------------------
		
        public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
		{
            Actions actionsToExecute = null;

            switch (this.source)
            {
                case Source.Actions:
                    actionsToExecute = this.actions;
                    break;

                case Source.Variable:
                    GameObject value = this.variable.Get(target) as GameObject;
                    if (value != null) actionsToExecute = value.GetComponent<Actions>();
                    break;
            }

            if (actionsToExecute != null)
			{
                this.actionsComplete = false;
                actionsToExecute.actionsList.Execute(target, this.OnCompleteActions);

                if (this.waitToFinish)
                {
                    WaitUntil wait = new WaitUntil(() =>
                    {
                        if (actionsToExecute == null) return true;
                        if (this.forceStop)
                        {
                            actionsToExecute.actionsList.Stop();
                            return true;
                        }

                        return this.actionsComplete;
                    });

                    yield return wait;
                }
			}

			yield return 0;
		}

        private void OnCompleteActions()
        {
            this.actionsComplete = true;
        }

        public override void Stop()
        {
            this.forceStop = true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

        #if UNITY_EDITOR

        public static new string NAME = "General/Execute Actions";
		private const string NODE_TITLE = "Execute actions {0} {1}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spSource;
        private SerializedProperty spActions;
        private SerializedProperty spVariable;

		private SerializedProperty spWaitToFinish;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
            string actionsName = (this.source == Source.Actions
                ? (this.actions == null ? "none" : this.actions.name)
                : this.variable.ToString()
            );

            return string.Format(
				NODE_TITLE,
                actionsName,
				(this.waitToFinish ? "and wait" : "")
			);
		}

		protected override void OnEnableEditorChild ()
		{
            this.spSource = this.serializedObject.FindProperty("source");
            this.spVariable = this.serializedObject.FindProperty("variable");
            this.spActions = this.serializedObject.FindProperty("actions");
			this.spWaitToFinish = this.serializedObject.FindProperty("waitToFinish");
		}

		protected override void OnDisableEditorChild ()
		{
            this.spSource = null;
            this.spVariable = null;
			this.spActions = null;
			this.spWaitToFinish = null;
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

			EditorGUILayout.PropertyField(this.spWaitToFinish);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}