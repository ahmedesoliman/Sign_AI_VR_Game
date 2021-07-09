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
	public class ActionListVariableLoop : IAction
	{
        public enum Source
        {
            Actions,
            Conditions,
            VariableWithActions,
            VariableWithConditions
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public HelperListVariable listVariables = new HelperListVariable();

        public Source source = Source.Actions;
        public Actions actions;
        public Conditions conditions;

        [VariableFilter(Variable.DataType.GameObject)]
        public VariableProperty variable = new VariableProperty(Variable.VarType.LocalVariable);

        private bool executionComplete = false;
        private bool forceStop = false;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
        {
            ListVariables list = this.listVariables.GetListVariables(target);
            if (list == null) yield break;

            Actions actionsToExecute = null;
            Conditions conditionsToExecute = null;

            switch (this.source)
            {
                case Source.Actions:
                    actionsToExecute = this.actions;
                    break;

                case Source.Conditions:
                    conditionsToExecute = this.conditions;
                    break;

                case Source.VariableWithActions:
                    GameObject valueActions = this.variable.Get(target) as GameObject;
                    if (valueActions != null) actionsToExecute = valueActions.GetComponent<Actions>();
                    break;

                case Source.VariableWithConditions:
                    GameObject valueConditions = this.variable.Get(target) as GameObject;
                    if (valueConditions != null) conditionsToExecute = valueConditions.GetComponent<Conditions>();
                    break;
            }

            for (int i = 0; i < list.variables.Count && !this.forceStop; ++i)
            {
                Variable itemVariable = list.Get(i);
                if (itemVariable == null) continue;

                GameObject itemGo = itemVariable.Get() as GameObject;
                if (itemGo == null) continue;

                this.executionComplete = false;
                if (actionsToExecute != null)
                {
                    actionsToExecute.actionsList.Execute(itemGo, this.OnCompleteActions);
                    WaitUntil wait = new WaitUntil(() =>
                    {
                        if (actionsToExecute == null) return true;
                        if (this.forceStop)
                        {
                            actionsToExecute.actionsList.Stop();
                            return true;
                        }

                        return this.executionComplete;
                    });

                    yield return wait;
                }
                else if (conditionsToExecute != null)
                {
                    conditionsToExecute.Interact(itemGo);
                }
            }

            yield return 0;
        }

        private void OnCompleteActions()
        {
            this.executionComplete = true;
        }

        public override void Stop()
        {
            base.Stop();
            this.forceStop = true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

        #if UNITY_EDITOR

        public static new string NAME = "Variables/List Variables Loop";
        private const string NODE_TITLE = "Loop List Variables {0}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spListVariables;
        private SerializedProperty spSource;
        private SerializedProperty spActions;
        private SerializedProperty spConditions;
        private SerializedProperty spVariable;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                this.listVariables
            );
        }

        protected override void OnEnableEditorChild()
        {
            this.spListVariables = this.serializedObject.FindProperty("listVariables");
            this.spSource = this.serializedObject.FindProperty("source");
            this.spVariable = this.serializedObject.FindProperty("variable");
            this.spActions = this.serializedObject.FindProperty("actions");
            this.spConditions = this.serializedObject.FindProperty("conditions");
        }

        protected override void OnDisableEditorChild()
        {
            this.spListVariables = null;
            this.spSource = null;
            this.spVariable = null;
            this.spActions = null;
            this.spConditions = null;
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spListVariables);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spSource);

            switch (this.spSource.enumValueIndex)
            {
                case (int)Source.Actions:
                    EditorGUILayout.PropertyField(this.spActions);
                    break;

                case (int)Source.Conditions:
                    EditorGUILayout.PropertyField(this.spConditions);
                    break;

                case (int)Source.VariableWithActions:
                    EditorGUILayout.PropertyField(this.spVariable);
                    break;

                case (int)Source.VariableWithConditions:
                    EditorGUILayout.PropertyField(this.spVariable);
                    break;
            }

            this.serializedObject.ApplyModifiedProperties();
        }

        #endif
    }
}
