namespace GameCreator.Variables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using GameCreator.Core;

    [CustomEditor(typeof(LocalVariables))]
    public class LocalVariablesEditor : GenericVariablesEditor<MBVariableEditor, MBVariable>
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        private LocalVariables instance;

		// INITIALIZERS: --------------------------------------------------------------------------

        protected override void OnEnable()
        {
            this.instance = (LocalVariables)this.target;
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

		// OVERRIDE METHODS: ----------------------------------------------------------------------

		public override void OnInspectorGUI()
		{
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Space();
            GlobalEditorID.Paint(this.instance);

            serializedObject.ApplyModifiedProperties();
		}

		protected override MBVariable[] GetReferences()
        {
            return this.instance.references;
        }
   
        protected override string GetReferenceName(int index)
        {
            if (index < 0 || index >= this.instance.references.Length)
            {
                return "<i>Unbound Variable</i>";
            }

            if (this.instance.references[index] == null)
            {
                return "<i>Undefined Variable</i>";
            }

            return this.subEditors[index].GetName();
        }

        protected override Variable.DataType GetReferenceType(int index)
        {
            if (index >= this.instance.references.Length) return Variable.DataType.Null;

            MBVariable reference = this.instance.references[index];
            if (reference == null) return Variable.DataType.Null;

            Variable variable = reference.variable;
            return (Variable.DataType)variable.type;
        }

        protected override bool MatchSearch(int index, string search, int tagsMask)
        {
            if (index >= this.subEditors.Length) return false;
            if (this.subEditors[index] == null) return false;

            return this.subEditors[index].MatchSearch(search, tagsMask);
        }

        protected override MBVariable CreateReferenceInstance(string name)
        {
            MBVariable variable = this.instance.gameObject.AddComponent<MBVariable>();
            variable.variable.name = name;
            if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(variable.gameObject.scene);
            return variable;
        }

		protected override void DeleteReferenceInstance(int index)
		{
            this.serializedObject.ApplyModifiedProperties();
            this.serializedObject.Update();

            MBVariable source = (MBVariable)this.spReferences
                .GetArrayElementAtIndex(index)
                .objectReferenceValue;

            this.spReferences.RemoveFromObjectArrayAt(index);
            this.RemoveSubEditorsElement(index);
            DestroyImmediate(source, true);

            this.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            this.serializedObject.Update();

            if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(this.instance.gameObject.scene);
        }

		protected override Tag[] GetReferenceTags(int index)
		{
            return new Tag[0];
		}

        // HIERARCHY CONTEXT MENU: ----------------------------------------------------------------

        [MenuItem("GameObject/Game Creator/Variables/Local Variables", false, 0)]
        public static void CreateLocalVariables()
        {
            GameObject instance = CreateSceneObject.Create("Local Variables");
            instance.AddComponent<LocalVariables>();
        }
	}
}