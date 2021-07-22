namespace GameCreator.Variables
{
    using System.IO;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using UnityEditor.AnimatedValues;
	using System.Linq;
	using System.Reflection;
    using GameCreator.Core;

	[CustomEditor(typeof(DatabaseVariables))]
	public class DatabaseVariablesEditor : IDatabaseEditor
	{
        private const string PROP_GLOBALTAGS = "tags";
        private const string PROP_GLOBALVARIABLES = "variables";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spTags;
        private SerializedProperty spVariables;
        private GlobalVariablesEditor variablesEditor;

		// INITIALIZE: ----------------------------------------------------------------------------

		private void OnEnable()
		{
            if (target == null || serializedObject == null) return;

            this.spTags = serializedObject.FindProperty(PROP_GLOBALTAGS);
            if (this.spTags.objectReferenceValue == null)
            {
                GameCreatorUtilities.CreateFolderStructure(GlobalTagsEditor.PATH_ASSET);
                GlobalTags instance = ScriptableObject.CreateInstance<GlobalTags>();
                AssetDatabase.CreateAsset(instance, Path.Combine(
                    GlobalTagsEditor.PATH_ASSET,
                    GlobalTagsEditor.NAME_ASSET
                ));

                this.spTags.objectReferenceValue = instance;
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
                serializedObject.Update();
            }

            this.spVariables = serializedObject.FindProperty(PROP_GLOBALVARIABLES);
            if (this.spVariables.objectReferenceValue == null)
            {
                GameCreatorUtilities.CreateFolderStructure(GlobalVariablesEditor.PATH_ASSET);
                GlobalVariables instance = ScriptableObject.CreateInstance<GlobalVariables>();
                AssetDatabase.CreateAsset(instance, Path.Combine(
                    GlobalVariablesEditor.PATH_ASSET, 
                    GlobalVariablesEditor.NAME_ASSET
                ));

                this.spVariables.objectReferenceValue = instance;
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
                serializedObject.Update();
            }

            this.variablesEditor = (GlobalVariablesEditor)CreateEditor(
                this.spVariables.objectReferenceValue
            ); 
		}

		// OVERRIDE METHODS: ----------------------------------------------------------------------

		public override string GetDocumentationURL ()
		{
			return "https://docs.gamecreator.io/manual/variables";
		}

		public override string GetName ()
		{
			return "Variables";
		}

        public override bool CanBeDecoupled()
        {
            return true;
        }

		// GUI METHODS: ---------------------------------------------------------------------------

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            if (this.variablesEditor != null)
            {
                EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins);
                this.variablesEditor.OnInspectorGUI();
                EditorGUILayout.EndVertical();
            }

			this.serializedObject.ApplyModifiedPropertiesWithoutUndo();
		}
	}
}