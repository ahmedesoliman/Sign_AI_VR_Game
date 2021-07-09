namespace GameCreator.Localization
{
	using System;
	using System.IO;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using UnityEditor.AnimatedValues;
	using UnityEditor.SceneManagement;
	using UnityEditorInternal;
	using System.Linq;
	using System.Reflection;
	using GameCreator.Core;

	[CustomEditor(typeof(DatabaseGeneral))]
	public class DatabaseGeneralEditor : IDatabaseEditor
	{
        private const string MSG_DP = "The default PlayerPrefs will be used if no Data Provider is selected";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spGeneralRenderMode;
        private SerializedProperty spPrefabFloatingMessage;
        private SerializedProperty spPrefabSimpleMessage;
        private SerializedProperty spPrefabTouchstick;
        private SerializedProperty spForceDisplayTouchstick;
        private SerializedProperty spSaveScenes;
        private SerializedProperty spProvider;
        private SerializedProperty spToolbarPositionX;
        private SerializedProperty spToolbarPositionY;

        private SerializedProperty spDefaultMusicAudioMixer;
        private SerializedProperty spDefaultSoundAudioMixer;
        private SerializedProperty spDefaultVoiceAudioMixer;

        private Editor editorDataProvider = null;

        // INITIALIZE: ----------------------------------------------------------------------------

        private void OnEnable()
		{
            if (target == null || serializedObject == null) return;
            this.spGeneralRenderMode = serializedObject.FindProperty("generalRenderMode");
            this.spPrefabFloatingMessage = serializedObject.FindProperty("prefabFloatingMessage");
            this.spPrefabSimpleMessage = serializedObject.FindProperty("prefabSimpleMessage");
            this.spPrefabTouchstick = serializedObject.FindProperty("prefabTouchstick");
            this.spForceDisplayTouchstick = serializedObject.FindProperty("forceDisplayInEditor");
            this.spSaveScenes = serializedObject.FindProperty("saveScenes");
            this.spProvider = serializedObject.FindProperty("provider");
            this.spToolbarPositionX = serializedObject.FindProperty("toolbarPositionX");
            this.spToolbarPositionY = serializedObject.FindProperty("toolbarPositionY");

            this.spDefaultMusicAudioMixer = serializedObject.FindProperty("musicAudioMixer");
            this.spDefaultSoundAudioMixer = serializedObject.FindProperty("soundAudioMixer");
            this.spDefaultVoiceAudioMixer = serializedObject.FindProperty("voiceAudioMixer");

            this.InitEditorDataProvider();
        }

        private void InitEditorDataProvider()
        {
            UnityEngine.Object dataProvider = this.spProvider.objectReferenceValue;
            if (dataProvider == null)
            {
                this.editorDataProvider = null;
                return;
            }

            this.editorDataProvider = Editor.CreateEditor(dataProvider);

        }

		// OVERRIDE METHODS: ----------------------------------------------------------------------

		public override string GetDocumentationURL ()
		{
			return "https://docs.gamecreator.io/";
		}

		public override string GetName ()
		{
			return "General";
		}

        public override int GetPanelWeight()
        {
            return 98;
        }

        public override bool CanBeDecoupled()
        {
            return true;
        }

        // GUI METHODS: ---------------------------------------------------------------------------

        public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spGeneralRenderMode);
            EditorGUILayout.PropertyField(this.spPrefabFloatingMessage);
            EditorGUILayout.PropertyField(this.spPrefabSimpleMessage);
            
            EditorGUILayout.PropertyField(this.spPrefabTouchstick);
            EditorGUILayout.PropertyField(this.spForceDisplayTouchstick);

            this.PaintProvider();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Audio Management", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(this.spDefaultMusicAudioMixer);
            EditorGUILayout.PropertyField(this.spDefaultSoundAudioMixer);
            EditorGUILayout.PropertyField(this.spDefaultVoiceAudioMixer);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Toolbar", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(this.spToolbarPositionX);
            EditorGUILayout.PropertyField(this.spToolbarPositionY);
            EditorGUI.indentLevel--;

            this.serializedObject.ApplyModifiedProperties();
		}

        private void PaintProvider()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Save/Load System:", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(this.spSaveScenes);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(this.spProvider, GUIContent.none);
            if (EditorGUI.EndChangeCheck()) this.InitEditorDataProvider();

            if (this.editorDataProvider != null)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                this.editorDataProvider.OnInspectorGUI();
                EditorGUILayout.EndVertical();
            }
            else
            {
                EditorGUILayout.HelpBox(MSG_DP, MessageType.Info);
            }
        }
	}
}