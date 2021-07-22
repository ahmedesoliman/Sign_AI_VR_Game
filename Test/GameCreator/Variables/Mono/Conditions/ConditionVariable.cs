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
    public abstract class ConditionVariable : ICondition
    {
        public enum Comparison
        {
            Equal,
            EqualInteger,
            Less,
            LessOrEqual,
            Greater,
            GreaterOrEqual
        }

        public Comparison comparison = Comparison.Equal;

        // EXECUTABLE: ----------------------------------------------------------------------------
        
        public override bool Check(GameObject target)
        {
            return this.Compare(target);
        }

        protected abstract bool Compare(GameObject target);

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

        #if UNITY_EDITOR

        public static new string NAME = "Variables/Variable";
        protected const string NODE_TITLE = "Compare {0} with {1}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spVariable;
        private SerializedProperty spCompareTo;
        private SerializedProperty spComparison;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return "unknown";
        }

        protected override void OnEnableEditorChild ()
        {
            this.spVariable = this.serializedObject.FindProperty("variable");
            this.spCompareTo = this.serializedObject.FindProperty("compareTo");
            this.spComparison = this.serializedObject.FindProperty("comparison");
        }

        protected virtual bool ShowComparison()
        {
            return false;
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();
            EditorGUILayout.PropertyField(this.spVariable);

            if (this.ShowComparison())
            {
                EditorGUILayout.PropertyField(this.spComparison);
            }

            EditorGUILayout.PropertyField(this.spCompareTo);
            this.serializedObject.ApplyModifiedProperties();
        }

        #endif
    }
}