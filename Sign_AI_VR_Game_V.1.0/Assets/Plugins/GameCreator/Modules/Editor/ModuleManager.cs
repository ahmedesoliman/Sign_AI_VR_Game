namespace GameCreator.ModuleManager
{
    using System;
    using System.IO;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Networking;
    using UnityEngine.Events;
    using UnityEditor;
    using GameCreator.Core;
    using System.Globalization;

    public abstract class ModuleManager
    {
        private const string ERR_DUPLICATE = "Duplicate module id {0}. Skipping initialization";
        private const string PROG_BACKUP_TITLE = "Creating Backup";
        private const string PROG_BACKUP_INFO = "Please, stand by...";
        private const string MSG_UPDATE_OK_TITLE = "The module has been successfully updated.";
        private const string MSG_UPDATE_OK_INFO = "The installer is no longer needed, though it is recommended that you keep it.";
        private const string MSG_UPDATE_FAIL_TITLE = "Oops! Something went wrong during the update:";
        private const string MSG_UPDATE_DATA_TITLE = "This update will override data from your project.";
        private const string MSG_UPDATE_DATA_INFO = "Are you sure you want to continue?";
        private const string MSG_UPDATE_DEPS_TITLE = "Unresolved module dependencies.";
        private const string MSG_UPDATE_DEPS_INFO = "This module can't be updated until all of its dependant modules are installed or updated.";
        private const string MSG_ENABLE_OK_TITLE = "The module has been successfully enabled.";
        private const string MSG_ENABLE_OK_INFO = "The installer is no longer needed, though it is recommended that you keep it.";
        private const string MSG_ENABLE_FAIL_TITLE = "Oops! Something went wrong during the activation:";
        private const string MSG_ENABLE_DEPS_TITLE = "Unresolved module dependencies.";
        private const string MSG_ENABLE_DEPS_INFO = "This module can't be enabled until all of its dependant modules are installed or updated.";
        private const string MSG_DISABLE_DEPS_TITLE = "Existing dependency doesn't allow to disable this module.";
        private const string MSG_DISABLE_DEPS_INFO = "This module can't be disabled until another active module that depend on this is disabled.";
        private const string MSG_DISABLE_CONFIRM_TITLE = "Disabling this module will permanently remove its data.";
        private const string MSG_DISABLE_CONFIRM_INFO = "This action can't be undone. Are you sure you want to continue?";
        private const string MSG_MISSING_ASSETMODULE_TITLE = "Unable to locate module {0} installer.";
        private const string MSG_MISSING_ASSETMODULE_INFO = "You can download it or remove the module information from the manifest.";

        private const string BACKUP_FILEPATH = "Backups/{0}/";
        private const string BACKUP_FILENAME = "{0}.unitypackage";
        private const string BACKUP_ASSETS_PATH = "Assets/Plugins/GameCreatorData";

        public const string ASSET_MODULES_PATH  = "Assets/Plugins/GameCreatorData/Modules/";
        public const string ASSET_PACK_FILENAME = "{0}.unitypackage";

        private const string ICON_PATH_ACT = "Assets/Plugins/GameCreator/Modules/Icons/UI/ModuleStateEnabled.png";
        private const string ICON_PATH_INS = "Assets/Plugins/GameCreator/Modules/Icons/UI/ModuleStateInstalled.png";
        private const string ICON_PATH_UPD = "Assets/Plugins/GameCreator/Modules/Icons/UI/ModuleStateUpdate.png";
        private const string ICON_PATH_STR = "Assets/Plugins/GameCreator/Modules/Icons/UI/ModuleStateStore.png";
        private const string ICON_PATH_UNK = "Assets/Plugins/GameCreator/Modules/Icons/UI/ModuleStateUnknown.png";

        private static Texture2D TEXTURE_MODULE_ACT;
        private static Texture2D TEXTURE_MODULE_INS;
        private static Texture2D TEXTURE_MODULE_UPD;
        private static Texture2D TEXTURE_MODULE_STR;
        private static Texture2D TEXTURE_MODULE_UNK;

        private static ModuleManifest CURRENT_MANIFEST = null;

        private static bool LOAD_DATA = true;
        private static Dictionary<string, ModuleManifest> PROJECT_MODULES = null;
        private static Dictionary<string, AssetModule> PROJECT_ASSET_MODULES = null;

        private static ModuleManifest[] PROJECT_MODULES_LIST = null;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public static void SetDirty()
        {
            LOAD_DATA = true;
            AssetDatabase.Refresh();
        }

        public static void Refresh()
        {
            ModuleManager.InitializeData();
        }

        public static ModuleManifest[] GetProjectManifests()
        {
            if (LOAD_DATA) ModuleManager.InitializeData();
            return ModuleManager.PROJECT_MODULES_LIST;
        }

        public static ModuleManifest GetModuleManifest(string moduleID)
        {
            if (LOAD_DATA) ModuleManager.InitializeData();
            if (ModuleManager.PROJECT_MODULES.ContainsKey(moduleID))
            {
                return ModuleManager.PROJECT_MODULES[moduleID];
            }

            return null;
        }

        public static Texture2D GetModuleIcon(string moduleID)
        {
            if (LOAD_DATA) ModuleManager.InitializeData();
            if (ModuleManager.PROJECT_MODULES.ContainsKey(moduleID))
            {
                ModuleManifest manifest = ModuleManager.PROJECT_MODULES[moduleID];
                AssetModule assetModule = null;
                if (ModuleManager.PROJECT_ASSET_MODULES.ContainsKey(moduleID))
                {
                    assetModule = ModuleManager.PROJECT_ASSET_MODULES[moduleID];
                }

                if (ModuleManager.IsEnabled(manifest.module))
                {
                    if (assetModule != null && assetModule.module.version.Higher(manifest.module.version))
                    {
                        return GetTextureModuleUpdate();
                    }

                    return GetTextureModuleEnabled();
                }

                return GetTextureModuleInstalled();
            }

            return GetTextureModuleUnknown();
        }

        public static bool IsUpdateAvailable(ModuleManifest manifest)
        {
            if (LOAD_DATA) ModuleManager.InitializeData();
            string moduleID = manifest.module.moduleID;

            if (ModuleManager.PROJECT_ASSET_MODULES.ContainsKey(moduleID))
            {
                Version manifestVersion = manifest.module.version;
                Version assetVersion = ModuleManager.PROJECT_ASSET_MODULES[moduleID].module.version;
                return assetVersion.Higher(manifestVersion);
            }

            return false;
        }

        public static AssetModule GetAssetModule(string moduleID)
        {
            if (LOAD_DATA) ModuleManager.InitializeData();
            if (!ModuleManager.PROJECT_ASSET_MODULES.ContainsKey(moduleID)) return null;
            return ModuleManager.PROJECT_ASSET_MODULES[moduleID];
        }

        public static bool IsEnabled(Module module)
        {
            if (LOAD_DATA) ModuleManager.InitializeData();
            if (module == null) return false;
            string absolutePath = ModuleManager.GetProjectPath();

            for (int i = 0; i < module.codePaths.Length; ++i)
            {
                string path = Path.Combine(absolutePath, module.codePaths[i]);
                if (Directory.Exists(path)) return true;
            }

            return false;
        }

        public static bool AssetModuleExists(Module module)
        {
            if (LOAD_DATA) ModuleManager.InitializeData();
            return (
                ModuleManager.PROJECT_ASSET_MODULES.ContainsKey(module.moduleID) &&
                ModuleManager.PROJECT_ASSET_MODULES[module.moduleID] != null
            );
        }

        public static string GetProjectPath()
        {
            return Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length);
        }

        public static void Backup(ModuleManifest manifest)
        {
            if (LOAD_DATA) ModuleManager.InitializeData();
            CURRENT_MANIFEST = manifest;
            EditorApplication.update += ModuleManager.DeferredBackup;
        }

        public static void Update(ModuleManifest manifest)
        {
            if (LOAD_DATA) ModuleManager.InitializeData();
            CURRENT_MANIFEST = manifest;
            EditorApplication.update += ModuleManager.DeferredUpdate;
        }

        public static void Enable(ModuleManifest manifest)
        {
            if (LOAD_DATA) ModuleManager.InitializeData();
            CURRENT_MANIFEST = manifest;
            EditorApplication.update += ModuleManager.DeferredEnable;
        }

        public static void Disable(ModuleManifest manifest)
        {
            if (LOAD_DATA) ModuleManager.InitializeData();
            CURRENT_MANIFEST = manifest;
            EditorApplication.update += ModuleManager.DeferredDisable;
        }

        public static void Remove(ModuleManifest manifest)
        {
            if (LOAD_DATA) ModuleManager.InitializeData();
            CURRENT_MANIFEST = manifest;
            EditorApplication.update += ModuleManager.DeferredRemove;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private static void InitializeData()
        {
            ModuleManager.LoadProjectModules();
            ModuleManager.LoadProjectAssetModules();
            ModuleManager.LoadProjectModulesList();
            ModuleManager.LOAD_DATA = false;
        }

        private static void LoadProjectModules()
        {
            PROJECT_MODULES = new Dictionary<string, ModuleManifest>();
            ModuleManifest[] manifests = AssetManifest.GetInstance().GetManifests();

            for (int i = 0; i < manifests.Length; ++i)
            {
                string mid = manifests[i].module.moduleID;
                if (string.IsNullOrEmpty(mid)) continue;

                if (PROJECT_MODULES.ContainsKey(mid))
                {
                    Debug.LogErrorFormat("Duplicate module id {0}. Skipping initialization", mid);
                    continue;
                }

                PROJECT_MODULES.Add(mid, manifests[i]);
            }
        }

        private static void LoadProjectAssetModules()
        {
            PROJECT_ASSET_MODULES = new Dictionary<string, AssetModule>();
            List<AssetModule> assetModules = ModuleManager.FindAssetsByType<AssetModule>();
            int assetModulesCount = assetModules.Count;

            for (int i = 0; i < assetModulesCount; ++i)
            {
                AssetModule assetModule = assetModules[i];
                if (PROJECT_ASSET_MODULES.ContainsKey(assetModule.module.moduleID))
                {
                    Debug.LogErrorFormat(ERR_DUPLICATE, assetModule.module.moduleID);
                    continue;
                }

                PROJECT_ASSET_MODULES.Add(assetModule.module.moduleID, assetModule);

                if (!PROJECT_MODULES.ContainsKey(assetModule.module.moduleID))
                {
                    PROJECT_MODULES.Add(assetModule.module.moduleID, new ModuleManifest(
                        assetModule.module
                    ));

                    ModuleManager.UpdateManifest(assetModule.module);
                }
            }
        }

        private static void LoadProjectModulesList()
        {
            PROJECT_MODULES_LIST = new ModuleManifest[PROJECT_MODULES.Count];
            int index = 0;
            foreach (KeyValuePair<string, ModuleManifest> kvMolduleManifest in PROJECT_MODULES)
            {
                PROJECT_MODULES_LIST[index] = kvMolduleManifest.Value;
                index++;
            }
        }

        private static void DeferredBackup()
        {
            EditorApplication.update -= DeferredBackup;

            string backupPath = string.Format(BACKUP_FILEPATH, DateTime.Now.ToString("yyyy-MM-dd"));
            string filePath = Path.Combine(ModuleManager.GetProjectPath(), backupPath);
            if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

            string fileName = string.Format(
                BACKUP_FILENAME,
                DateTime.Now.ToString("hh-mm-ss")
            );

            string[] assetsGUID = AssetDatabase.FindAssets("", new string[] { BACKUP_ASSETS_PATH });
            string[] assetsPath = new string[assetsGUID.Length];
            for (int i = 0; i < assetsGUID.Length; ++i)
            {
                assetsPath[i] = AssetDatabase.GUIDToAssetPath(assetsGUID[i]);
            }

            AssetDatabase.ExportPackage(
                assetsPath,
                Path.Combine(filePath, fileName),
                ExportPackageOptions.Default
            );

            string showPath = Path.Combine(
                Directory.GetParent(Application.dataPath).FullName, 
                backupPath
            );

            EditorUtility.RevealInFinder(showPath);
        }

        private static void DeferredUpdate()
        {
            EditorApplication.update -= DeferredUpdate;

            if (ModuleManager.CURRENT_MANIFEST == null) return;
            ModuleManifest manifest = ModuleManager.CURRENT_MANIFEST;
            AssetModule assetModule = ModuleManager.PROJECT_ASSET_MODULES[manifest.module.moduleID];

            if (!ModuleManager.DependenciesResolved(manifest.module))
            {
                EditorUtility.DisplayDialog(MSG_UPDATE_DEPS_TITLE, MSG_UPDATE_DEPS_INFO, "Ok");
                return;
            }

            if (assetModule.module.includesData)
            {
                bool accept = EditorUtility.DisplayDialog(MSG_UPDATE_DATA_TITLE, MSG_UPDATE_DATA_INFO, "Yes", "Cancel");
                if (!accept) return;
            }

            ModuleManager.DeleteModuleContent(manifest.module, false);
            ModuleManager.ImportModuleContent(assetModule, CallbackUpdateComplete);
        }

        private static void DeferredEnable()
        {
            EditorApplication.update -= DeferredEnable;

            if (ModuleManager.CURRENT_MANIFEST == null) return;
            ModuleManifest manifest = ModuleManager.CURRENT_MANIFEST;
            if (!ModuleManager.AssetModuleExists(manifest.module))
            {
                int option = EditorUtility.DisplayDialogComplex(
                    string.Format(MSG_MISSING_ASSETMODULE_TITLE, manifest.module.moduleID),
                    MSG_MISSING_ASSETMODULE_INFO,
                    "Download",
                    "Clean Manifest",
                    "Cancel"
                );

                switch (option)
                {
                    case 0:
                        Application.OpenURL("https://hub.gamecreator.io/content/modules");
                        break;

                    case 1:
                        ModuleManager.RemoveManifest(manifest.module);
                        ModuleManager.SetDirty();
                        break;
                }

                return;
            }

            AssetModule assetModule = ModuleManager.PROJECT_ASSET_MODULES[manifest.module.moduleID];
            if (!ModuleManager.DependenciesResolved(assetModule.module))
            {
                EditorUtility.DisplayDialog(MSG_ENABLE_DEPS_TITLE, MSG_ENABLE_DEPS_INFO, "Ok");
                return;
            }

            ModuleManager.ImportModuleContent(assetModule, CallbackEnableComplete);
        }

        private static void DeferredDisable()
        {
            EditorApplication.update -= DeferredDisable;

            if (ModuleManager.CURRENT_MANIFEST == null) return;
            ModuleManifest manifest = ModuleManager.CURRENT_MANIFEST;

            ModuleManifest[] allManifests = ModuleManager.GetProjectManifests();
            bool hasDependency = false;

            for (int i = 0; !hasDependency && i < allManifests.Length; ++i)
            {
                if (allManifests[i].module.moduleID == manifest.module.moduleID) continue;
                if (!ModuleManager.IsEnabled(allManifests[i].module)) continue;

                for (int j = 0; !hasDependency && j < allManifests[i].module.dependencies.Length; ++j)
                {
                    if (allManifests[i].module.dependencies[j].moduleID == manifest.module.moduleID)
                    {
                        hasDependency = true;
                    }
                }
            }

            if (hasDependency)
            {
                if (EditorUtility.DisplayDialog(MSG_DISABLE_DEPS_TITLE, MSG_DISABLE_DEPS_INFO, "Ok"))
                {
                    return;
                }
            }

            if (EditorUtility.DisplayDialog(
                MSG_DISABLE_CONFIRM_TITLE, 
                MSG_DISABLE_CONFIRM_INFO, 
                "Yes", 
                "Cancel"))
            {
                ModuleManager.RemoveManifest(manifest.module);
                ModuleManager.DeleteModuleContent(manifest.module, true);
                ModuleManager.SetDirty();
            }
        }

        private static void DeferredRemove()
        {
            EditorApplication.update -= DeferredRemove;

            if (ModuleManager.CURRENT_MANIFEST == null) return;
            ModuleManifest manifest = ModuleManager.CURRENT_MANIFEST;
            if (!ModuleManager.PROJECT_ASSET_MODULES.ContainsKey(manifest.module.moduleID)) return;
            AssetModule assetModule = ModuleManager.PROJECT_ASSET_MODULES[manifest.module.moduleID];

            ModuleManager.RemoveManifest(manifest.module);
            ModuleManager.DeleteModuleAsset(assetModule);
            ModuleManager.SetDirty();
        }

        // PRIVATE CALLBACK METHODS: --------------------------------------------------------------

        private static void CallbackUpdateComplete(string moduleID)
        {
            AssetDatabase.importPackageCompleted -= ModuleManager.CallbackUpdateComplete;
            bool keepInstaller = EditorUtility.DisplayDialog(
                MSG_UPDATE_OK_TITLE, 
                MSG_UPDATE_OK_INFO,
                "Keep it",
                "Remove installer"
            );

            if (LOAD_DATA) ModuleManager.InitializeData();
            AssetModule assetModule = ModuleManager.PROJECT_ASSET_MODULES[moduleID];
            if (!keepInstaller) ModuleManager.DeleteModuleAsset(assetModule);

            ModuleManager.UpdateManifest(assetModule.module);
            ModuleManager.SetDirty();
        }

        private static void CallbackEnableComplete(string moduleID)
        {
            AssetDatabase.importPackageCompleted -= ModuleManager.CallbackEnableComplete;
            bool keepInstaller = EditorUtility.DisplayDialog(
                MSG_ENABLE_OK_TITLE,
                MSG_ENABLE_OK_INFO,
                "Keep it",
                "Remove installer"
            );

            if (LOAD_DATA) ModuleManager.InitializeData();
            AssetModule assetModule = ModuleManager.PROJECT_ASSET_MODULES[moduleID];
            if (!keepInstaller) ModuleManager.DeleteModuleAsset(assetModule);

            ModuleManager.UpdateManifest(assetModule.module);
            ModuleManager.SetDirty();
        }

        // PRIVATE UTILITIES METHODS: -------------------------------------------------------------

        private static List<T> FindAssetsByType<T>() where T : UnityEngine.ScriptableObject
        {
            List<T> assets = new List<T>();
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));

            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }

            return assets;
        }

        private static void UpdateManifest(Module module)
        {
            if (module == null) return;
            AssetManifest.GetInstance().UpdateManifest(module);
        }

        private static void RemoveManifest(Module module)
        {
            if (module == null) return;
            AssetManifest.GetInstance().RemoveModule(module);
        }

        private static void DeleteModuleAsset(AssetModule assetModule)
        {
            string absolutePath = ModuleManager.GetProjectPath();
            string relativePackagePath = Path.Combine(ASSET_MODULES_PATH, assetModule.module.moduleID);
            string absolutePackagePath = Path.Combine(absolutePath, relativePackagePath);

            if (Directory.Exists(absolutePackagePath))
            {
                FileUtil.DeleteFileOrDirectory(absolutePackagePath);
            }
        }

        private static void DeleteModuleContent(Module module, bool removeData = false)
        {
            List<string> paths = new List<string>(module.codePaths);
            if (removeData) paths.AddRange(module.dataPaths);

            for (int i = 0; i < paths.Count; ++i)
            {
                if (string.IsNullOrEmpty(paths[i])) continue;
                FileUtil.DeleteFileOrDirectory(paths[i]);
            }
        }

        private static void ImportModuleContent(AssetModule assetModule, AssetDatabase.ImportPackageCallback onSuccess)
        {
            string packageFilename = string.Format(ASSET_PACK_FILENAME, assetModule.module.moduleID);
            string relativePackagePath = Path.Combine(ASSET_MODULES_PATH, assetModule.module.moduleID);
            relativePackagePath = Path.Combine(relativePackagePath, packageFilename);
            string absolutePackagePath = Path.Combine(ModuleManager.GetProjectPath(), relativePackagePath);

            if (File.Exists(absolutePackagePath))
            {
                AssetDatabase.importPackageCompleted += onSuccess;
                AssetDatabase.ImportPackage(relativePackagePath, true);
            }
        }

        public static bool DependenciesResolved(Module module)
        {
            if (LOAD_DATA) ModuleManager.InitializeData();
            for (int i = 0; i < module.dependencies.Length; ++i)
            {
                string depModuleID = module.dependencies[i].moduleID;
                Version depVersion = module.dependencies[i].version;

                if (!ModuleManager.PROJECT_MODULES.ContainsKey(depModuleID) ||
                    depVersion.Higher(ModuleManager.PROJECT_MODULES[depModuleID].module.version) ||
                    !ModuleManager.IsEnabled(ModuleManager.PROJECT_MODULES[depModuleID].module))
                {
                    return false;
                }
            }

            return true;
        }

        // PRIVATE TEXTURE METHODS: ---------------------------------------------------------------

        private static Texture2D GetTextureModuleEnabled()
        {
            if (TEXTURE_MODULE_ACT == null)
            {
                TEXTURE_MODULE_ACT = AssetDatabase.LoadAssetAtPath<Texture2D>(ICON_PATH_ACT);
            }

            return TEXTURE_MODULE_ACT;
        }

        private static Texture2D GetTextureModuleInstalled()
        {
            if (TEXTURE_MODULE_INS == null)
            {
                TEXTURE_MODULE_INS = AssetDatabase.LoadAssetAtPath<Texture2D>(ICON_PATH_INS);
            }

            return TEXTURE_MODULE_INS;
        }

        private static Texture2D GetTextureModuleUpdate()
        {
            if (TEXTURE_MODULE_UPD == null)
            {
                TEXTURE_MODULE_UPD = AssetDatabase.LoadAssetAtPath<Texture2D>(ICON_PATH_UPD);
            }

            return TEXTURE_MODULE_UPD;
        }

        private static Texture2D GetTextureModuleStore()
        {
            if (TEXTURE_MODULE_STR == null)
            {
                TEXTURE_MODULE_STR = AssetDatabase.LoadAssetAtPath<Texture2D>(ICON_PATH_STR);
            }

            return TEXTURE_MODULE_STR;
        }

        private static Texture2D GetTextureModuleUnknown()
        {
            if (TEXTURE_MODULE_UNK == null)
            {
                TEXTURE_MODULE_UNK = AssetDatabase.LoadAssetAtPath<Texture2D>(ICON_PATH_UNK);
            }

            return TEXTURE_MODULE_UNK;
        }
    }
}