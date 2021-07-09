namespace GameCreator.Variables
{
    using System.IO;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    public class TagsEditorWindow : PopupWindowContent
    {
        private const string TITLE = "Tag Settings";

        // PUBLIC METHODS: ------------------------------------------------------------------------

        private GlobalTagsEditor tagsEditor;
        private Vector2 scroll = Vector2.zero;

        // INITIALIZERS: --------------------------------------------------------------------------

        public override void OnOpen()
        {
            GlobalTags instance = AssetDatabase.LoadAssetAtPath<GlobalTags>(Path.Combine(
                GlobalTagsEditor.PATH_ASSET,
                GlobalTagsEditor.NAME_ASSET
            ));

            if (instance == null) this.editorWindow.Close();
            this.tagsEditor = (GlobalTagsEditor)Editor.CreateEditor(instance);
        }

        public override void OnClose()
        {

        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(250, 500);
        }

        public override void OnGUI(Rect rect)
        {
            if (this.tagsEditor == null) return;

            this.scroll = EditorGUILayout.BeginScrollView(
                this.scroll, 
                EditorStyles.inspectorDefaultMargins
            );

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(TITLE, EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.Space();

            this.tagsEditor.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.EndScrollView();
        }
    }
}