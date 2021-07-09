using System.IO;
namespace GameCreator.Update
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using UnityEditorInternal;

    [CustomEditor(typeof(Config))]
    public class ConfigEditor : Editor 
    {
        private const string GC_VERSION = "Version {0}";
        private const string GC_REMOVE = "Remove Folders";
        private const string GC_BUILD = "Build Folders";
        private const string GC_BUILD_PACKAGE = "Build Package";

        private const string EXPORT_PATH = "Plugins/GameCreatorUpdate/Data/Package.unitypackage";

        // PROPERTIES: ----------------------------------------------------------------------------

        public SerializedProperty spVersionMajor;
        public SerializedProperty spVersionMinor;
        public SerializedProperty spVersionPatch;

        public SerializedProperty spInteractiveInstall;

        private ReorderableList removeDirectoriesList;
        private SerializedProperty spRemoveDirectories;

        private ReorderableList buildDirectoriesList;
        private SerializedProperty spBuildDirectories;

        private Config config = null;
        private bool openToEdit = false;

        // INITIALIZERS: --------------------------------------------------------------------------

        private void OnEnable()
        {
            this.config = this.target as Config;

            SerializedProperty spVersion = serializedObject.FindProperty("version");
            this.spVersionMajor = spVersion.FindPropertyRelative("major");
            this.spVersionMinor = spVersion.FindPropertyRelative("minor");
            this.spVersionPatch = spVersion.FindPropertyRelative("patch");

            this.spInteractiveInstall = serializedObject.FindProperty("interactiveInstall");

            this.spRemoveDirectories = serializedObject.FindProperty("removeDirectories");
            this.removeDirectoriesList = new ReorderableList(
                this.serializedObject,
                this.spRemoveDirectories,
                true, true, true, true
            );

            this.removeDirectoriesList.drawHeaderCallback = (Rect r) => EditorGUI.LabelField(r, GC_REMOVE);

            this.removeDirectoriesList.drawElementCallback = (Rect rect, int i, bool a, bool f) => {
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y + 1f, rect.width, rect.height - 4f),
                    this.spRemoveDirectories.GetArrayElementAtIndex(i),
                    GUIContent.none
                );
            };

            this.spBuildDirectories = serializedObject.FindProperty("buildDirectories");
            this.buildDirectoriesList = new ReorderableList(
                this.serializedObject,
                this.spBuildDirectories,
                true, true, true, true
            );

            this.buildDirectoriesList.drawHeaderCallback = (Rect r) => EditorGUI.LabelField(r, GC_BUILD);

            this.buildDirectoriesList.drawElementCallback = (Rect rect, int i, bool a, bool f) => {
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y + 1f, rect.width, rect.height - 4f),
                    this.spBuildDirectories.GetArrayElementAtIndex(i),
                    GUIContent.none
                );
            };
        }

        // PAINT METHOD: --------------------------------------------------------------------------

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            switch (this.openToEdit)
            {
                case true: this.PaintEditMode(); break;
                case false: this.PaintNormalMode(); break;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void PaintNormalMode()
        {
            EditorGUILayout.HelpBox(string.Format(GC_VERSION, this.config.version), MessageType.Info);

            if (GUILayout.Button("Edit", EditorStyles.miniButton))
            {
                this.openToEdit = true;
            }
        }

        private void PaintEditMode()
        {
            EditorGUILayout.LabelField("Version", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(this.spVersionMajor);
            EditorGUILayout.PropertyField(this.spVersionMinor);
            EditorGUILayout.PropertyField(this.spVersionPatch);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spInteractiveInstall);

            EditorGUILayout.Space();
            this.removeDirectoriesList.DoLayoutList();

            EditorGUILayout.Space();
            this.buildDirectoriesList.DoLayoutList();

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(GC_BUILD_PACKAGE, EditorStyles.miniButtonLeft))
            {
                if (this.config == null) return;

                string[] buildDirectories = new string[this.config.buildDirectories.Length];
                for (int i = 0; i < this.config.buildDirectories.Length; ++i)
                {
                    buildDirectories[i] = this.config.buildDirectories[i].TrimEnd('/');
                }

                string[] assetsGuids = AssetDatabase.FindAssets("", buildDirectories);
                string[] assetsPaths = new string[assetsGuids.Length];

                for (int i = 0; i < assetsGuids.Length; ++i)
                {
                    assetsPaths[i] = AssetDatabase.GUIDToAssetPath(assetsGuids[i]);
                }

                AssetDatabase.ExportPackage(
                    assetsPaths,
                    Path.Combine(Application.dataPath, EXPORT_PATH),
                    ExportPackageOptions.Default
                );
                    
                AssetDatabase.Refresh();

                EditorUtility.CopySerialized(
                    Config.GetUpdate(), 
                    Config.GetCurrent()
                );
            }

            if (GUILayout.Button("Exit Edit Mode", EditorStyles.miniButtonRight))
            {
                this.openToEdit = false;
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}