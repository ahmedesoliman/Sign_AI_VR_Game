namespace GameCreator.ModuleManager
{
    using System;
    using System.IO;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    [System.Serializable]
    [CreateAssetMenu(
        fileName = "Module Manifest",
        menuName = "Game Creator/Developer/Module Manifest",
        order = 200
    )]
    public class AssetModule : ScriptableObject
    {
        private const string MSG_BUILD_TITLE = "Building a module will remove the current unity package";
        private const string MSG_BUILD_INFO = "Are you sure you want to continue?";

        // PROPERTIES: ----------------------------------------------------------------------------

        public Module module = new Module();

        public bool adminLogin = false;
        public bool adminOpen = false;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void BuildModule()
        {
            if (this.module == null || string.IsNullOrEmpty(this.module.moduleID))
            {
                Debug.LogError("Unable to build module. Module is null or empty moduleID");
                return;
            }

            if (!EditorUtility.DisplayDialog(MSG_BUILD_TITLE, MSG_BUILD_INFO, "Yes", "Cancel"))
            {
                return;
            }

            List<string> paths = new List<string>(this.module.codePaths);
            if (module.includesData) paths.AddRange(this.module.dataPaths);
            for (int i = 0; i < paths.Count; ++i)
            {
                paths[i] = Path.GetDirectoryName(paths[i]);
            }

            string packageFilename = string.Format(ModuleManager.ASSET_PACK_FILENAME, this.module.moduleID);
            string relativePackagePath = Path.Combine(ModuleManager.ASSET_MODULES_PATH, this.module.moduleID);
            relativePackagePath = Path.Combine(relativePackagePath, packageFilename);
            string absolutePackagePath = Path.Combine(ModuleManager.GetProjectPath(), relativePackagePath);

            UnityEngine.Object otherPackage = AssetDatabase.LoadMainAssetAtPath(relativePackagePath);
            if (otherPackage != null) AssetDatabase.DeleteAsset(relativePackagePath);

            AssetDatabase.ExportPackage(
                paths.ToArray(), 
                absolutePackagePath, 
                ExportPackageOptions.Recurse
            );

            AssetDatabase.Refresh();
        }
    }
}