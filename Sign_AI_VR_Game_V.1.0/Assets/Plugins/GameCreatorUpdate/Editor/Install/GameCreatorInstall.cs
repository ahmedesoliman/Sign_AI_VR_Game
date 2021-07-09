namespace GameCreator.Update
{
    using System;
    using System.IO;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.SceneManagement;

    public static class GameCreatorInstall
    {
        public class Requirement
        {
            public bool success;
            public string message;

            public Requirement(bool success, string message = "")
            {
                this.success = success;
                this.message = message;
            }
        }

        public const string ASSETS_PATH = "Assets/";
        public const string PACKAGE_PATH = "Plugins/GameCreatorUpdate/Data/Package.unitypackage";
        public const string CONFIG_PATH = "Plugins/GameCreatorUpdate/Data/Config.asset";

        private const string MSG_INSTALL1 = "Installing Game Creator {0}";
        private const string MSG_INSTALL2 = "This process should take less than a few minutes...";

        private const string MSG_COMPLETE1 = "Update Complete!";
        private const string MSG_COMPLETE2 = "Game Creator has been updated to version {0}";

        public const string REQUIREMENTS_TITLE = "Game Creator can not be installed";

        public const string MAGIC_CODE = "yo2w4kjYXvSDilkVauDX";

        // INITIALIZE METHODS: --------------------------------------------------------------------

        [InitializeOnLoadMethod]
        static void OnInitializeInstall()
        {
            if (EditorApplication.isPlaying) return;
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

            EditorApplication.update += PrepareInstallUpdate;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public static void PrepareInstallUpdate()
        {
            if (EditorApplication.isCompiling) return;
            if (EditorApplication.isUpdating) return;

            EditorApplication.update -= PrepareInstallUpdate;

            Version updateVersion = Config.GetUpdate().version;
            Version currentVersion = Config.GetCurrent().version;

            if (updateVersion.Equals(Version.NONE)) return;
            if (!updateVersion.HigherThan(currentVersion)) return;

            GameCreatorInstallWindow.OpenWindow();
        }

        public static void InstallUpdate()
        {
            Requirement requirements = MeetsUnityRequirements();
            if (!requirements.success)
            {
                EditorUtility.DisplayDialog(REQUIREMENTS_TITLE, requirements.message, "Ok");
                return;
            }

            Debug.Log("Preparing installation...");
            if (!File.Exists(Path.Combine(Application.dataPath, PACKAGE_PATH)))
            {
                string path = Path.Combine(Application.dataPath, PACKAGE_PATH);
                Debug.LogError("Unable to locate Package file at: " + path);
                return;
            }

            if (!File.Exists(Path.Combine(Application.dataPath, CONFIG_PATH)))
            {
                string path = Path.Combine(Application.dataPath, CONFIG_PATH);
                Debug.LogError("Unable to locate Config file at: " + path);
                return;
            }

            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            SceneSetup[] scenesSetup = EditorSceneManager.GetSceneManagerSetup();
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            string[] removeFolders = Config.GetUpdate().removeDirectories;
            for (int i = 0; i < removeFolders.Length; ++i)
            {
                string assetPath = Path.Combine(Application.dataPath, removeFolders[i]);
                if (File.Exists(assetPath) || Directory.Exists(assetPath))
                {
                    FileUtil.DeleteFileOrDirectory(assetPath);
                }
            }

            Debug.Log("Installing Game Creator...");
            AssetDatabase.ImportPackage(
                Path.Combine(ASSETS_PATH, PACKAGE_PATH),
                Config.GetUpdate().interactiveInstall
            );

            Debug.Log("<b>Installation Complete</b>!");
            if (scenesSetup != null && scenesSetup.Length > 0)
            {
                EditorSceneManager.RestoreSceneManagerSetup(scenesSetup);
            }

            EditorUtility.CopySerialized(Config.GetUpdate(), Config.GetCurrent());

            EditorUtility.DisplayDialog(
                MSG_COMPLETE1,
                string.Format(MSG_COMPLETE2, Config.GetUpdate().version),
                "Ok"
            );
        }

        // REQUIREMENTS: ------------------------------------------------------------------------------

        private const string ERR_OLD = "Your Unity version is older than 2019.4, which is no longer officially supported.";
        private const string ERR_ALPHA = "Your Unity version is an Alpha, Beta or Tech-Stream and might be unstable.";

        private const string ERR_LTS = "Please, consider using the latest Unity 2019.4 LTS version";

        public static Requirement MeetsUnityRequirements()
        {
            bool success = true;
            string message = string.Empty;

            #if UNITY_2020_1_OR_NEWER
            success = false;
            message = string.Format("{0} {1}", ERR_ALPHA, ERR_LTS);
            #elif UNITY_2019_4_OR_NEWER
            success = true;
            message = string.Empty;
            #else
            success = false;
            message = string.Format("{0} {1}", ERR_OLD, ERR_LTS);
            #endif

            return new Requirement(success, message);
        }
    }
}