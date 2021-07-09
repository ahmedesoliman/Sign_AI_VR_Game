namespace GameCreator.ModuleManager
{
    using System;
    using System.IO;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using GameCreator.Core;

    [Serializable]
    public class AssetManifest : ScriptableObject
    {
        private const string RELATIVE_PATH = "Assets/{0}/{1}";
        private const string ASSET_PATH = "Plugins/GameCreatorData/Assets";
        private const string ASSET_NAME = "Manifest.asset";

        private const string PROP_MANIFESTS = "manifests";
        private const string PROP_IS_ENABLED = "isEnabled";
        private const string PROP_MODULE = "module";

        private static AssetManifest Instance;

        // PROPERTIES: ----------------------------------------------------------------------------

        public ModuleManifest[] manifests = new ModuleManifest[0];

        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public static AssetManifest GetInstance()
        {
            if (AssetManifest.Instance != null) return AssetManifest.Instance;

            AssetManifest manifest;
            string absPath = Path.Combine(Application.dataPath, Path.Combine(ASSET_PATH, ASSET_NAME));
            string relPath = string.Format(RELATIVE_PATH, ASSET_PATH, ASSET_NAME);

            if (File.Exists(absPath))
            {
                manifest = AssetDatabase.LoadAssetAtPath<AssetManifest>(relPath);
            }
            else
            {
                string dirPath = string.Format(RELATIVE_PATH, ASSET_PATH, "");
                GameCreatorUtilities.CreateFolderStructure(dirPath);

                manifest = ScriptableObject.CreateInstance<AssetManifest>();
                AssetDatabase.CreateAsset(manifest, relPath);
            }

            AssetManifest.Instance = manifest;
            return AssetManifest.Instance;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public ModuleManifest[] GetManifests()
        {
            return this.manifests;
        }

        public void UpdateManifest(Module module)
        {
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty propManifests = serializedObject.FindProperty(PROP_MANIFESTS);

            int manifestIndex = this.GetManifestIndex(module);
            if (manifestIndex >= 0)
            {
                propManifests = propManifests.GetArrayElementAtIndex(manifestIndex);
                Module.UpdateModule(propManifests.FindPropertyRelative(PROP_MODULE), module);
            }
            else
            {
                manifestIndex = propManifests.arraySize;
                propManifests.InsertArrayElementAtIndex(manifestIndex);
                propManifests = propManifests.GetArrayElementAtIndex(manifestIndex);
                Module.UpdateModule(propManifests.FindPropertyRelative(PROP_MODULE), module);
            }

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        public void RemoveModule(Module module)
        {
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty propManifests = serializedObject.FindProperty(PROP_MANIFESTS);

            int manifestIndex = this.GetManifestIndex(module);
            if (manifestIndex >= 0)
            {
                propManifests.DeleteArrayElementAtIndex(manifestIndex);
            }

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private int GetManifestIndex(Module module)
        {
            for (int i = 0; i < this.manifests.Length; ++i)
            {
                if (this.manifests[i].module.moduleID == module.moduleID) return i;
            }

            return -1;
        }
    }
}