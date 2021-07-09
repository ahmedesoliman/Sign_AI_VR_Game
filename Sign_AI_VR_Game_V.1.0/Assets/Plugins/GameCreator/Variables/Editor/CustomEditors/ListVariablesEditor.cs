using System.Runtime.CompilerServices;
namespace GameCreator.Variables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using GameCreator.Core;

    [CustomEditor(typeof(ListVariables))]
    public class ListVariablesEditor : LocalVariablesEditor
    {
        private const string RUNTIME_NAME = "{0}{1}";
        private const string REFERENCE_NAME = "Item: {0} {1}";
        private const string SAVE = "(save)";

        // PROPERTIES: ----------------------------------------------------------------------------

        private ListVariables list;

        private SerializedProperty spType;
        private SerializedProperty spSave;

        // INITIALIZERS: --------------------------------------------------------------------------

        protected override void OnEnable()
        {
            base.OnEnable();

            this.list = this.target as ListVariables;
            this.spType = this.serializedObject.FindProperty("type");
            this.spSave = this.serializedObject.FindProperty("save");
        }

        // PAINT METHODS: -------------------------------------------------------------------------

        public override void OnInspectorGUI()
        {
            if (Application.isPlaying) this.PaintRuntimeInspector(); 
            else base.OnInspectorGUI();
        }

        private void PaintRuntimeInspector()
        {
            EditorGUILayout.Space();

            if (this.list.variables.Count == 0)
            {
                EditorGUILayout.HelpBox("Empty List", MessageType.Info);
                return;
            }

            GUILayoutOption width = GUILayout.Width(150);
            GUILayoutOption height = GUILayout.Height(20);
            EditorGUI.BeginDisabledGroup(true);

            for (int i = 0; i < this.list.variables.Count; ++i)
            {
                object variable = this.list.variables[i].Get();
                string title = string.Format(
                    RUNTIME_NAME,
                    this.list.iterator == i ? " ▸ " : string.Empty,
                    string.Format(REFERENCE_NAME, i, string.Empty)
                );

                string value = (variable == null 
                    ? "(null)" 
                    : variable.ToString()
                );

                EditorGUILayout.BeginHorizontal();

                GUILayout.Button(title, CoreGUIStyles.GetToggleButtonLeftOff(), height, width);
                GUILayout.Button(value, CoreGUIStyles.GetToggleButtonRightOn(), height);

                EditorGUILayout.EndHorizontal();
            }

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space();

            serializedObject.Update();
            GlobalEditorID.Paint(this.list);
            serializedObject.ApplyModifiedProperties();
        }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        protected override void PaintCreateVariable(bool usingSearch)
        {
            if (usingSearch) return;

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(this.spType, GUIContent.none, GUILayout.Width(150));
            if (EditorGUI.EndChangeCheck())
            {
                for (int i = 0; i < this.subEditors.Length; ++i)
                {
                    this.subEditors[i].serializedObject.ApplyModifiedProperties();
                    this.subEditors[i].serializedObject.Update();

                    this.subEditors[i].spVariableType.intValue = this.spType.intValue;

                    this.subEditors[i].serializedObject.ApplyModifiedPropertiesWithoutUndo();
                    this.subEditors[i].serializedObject.Update();
                }
            }

            if (!this.CanSave() && this.spSave.boolValue) this.spSave.boolValue = false;
            EditorGUI.BeginDisabledGroup(!this.CanSave());

            this.spSave.boolValue = EditorGUILayout.ToggleLeft(
                this.spSave.displayName, 
                this.spSave.boolValue
            );

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
        }

        protected override string GetReferenceName(int index)
        {
            return string.Format(
                REFERENCE_NAME, 
                index, 
                this.spSave.boolValue ? SAVE : string.Empty
            );
        }

        protected override void BeforePaintSubEditor(int index)
        {
            this.subEditors[index].editableType = false;
            this.subEditors[index].editableCommon = false;
        }

        protected override void AfterPaintSubEditorsList()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("+", GUILayout.Height(20), GUILayout.Width(100)))
            {
                string variableName = Guid.NewGuid().ToString("N");
                MBVariable variable = this.CreateVariable(variableName);

                variable.variable.type = this.spType.intValue;
                Event.current.Use();
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private bool CanSave()
        {
            Variable.DataType type = (Variable.DataType)this.spType.enumValueIndex;
            return Variable.CanSave(type);
        }

        // HIERARCHY CONTEXT MENU: ----------------------------------------------------------------

        [MenuItem("GameObject/Game Creator/Variables/List Variables", false, 0)]
        public static void CreateListVariables()
        {
            GameObject instance = CreateSceneObject.Create("List Variables");
            instance.AddComponent<ListVariables>();
        }
	}
}