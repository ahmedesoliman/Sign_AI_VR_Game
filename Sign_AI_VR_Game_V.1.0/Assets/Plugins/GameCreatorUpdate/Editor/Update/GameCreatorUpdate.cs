namespace GameCreator.Update
{
    using System;
    using System.IO;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    public static class GameCreatorUpdate
    {
        public const string KEY_AUTO_CHECK_UPDATES = "gamecreator-update-autocheck";
        public const string KEY_SKIP_PACKAGE_VERSION = "gamecreator-skip-version";
        public const string KEY_LAST_CHECK_DATE = "gamecreator-update-checkdate";
        public const string KEY_UPDATE_CACHE = "gamecreator-update-cache";

        public const string GAMECREATOR_BUNDLE = "com.gamecreator.module.core";
        public const string URL_DOWNLOAD = "https://hub.gamecreator.io/downloads";
        public static bool CHECKING_UPDATES = false;

        private const string PATCH = "This version squashes various bugs and fixes some minor issues.";
        private const string RELEASE = "This version brings brand-new features but might cause some conflicts with existing projects";
        private const string ALPHA = "This version brings experimental features and is not meant to be used for production";

        // INITIALIZE METHODS: --------------------------------------------------------------------

        [InitializeOnLoadMethod]
        static void OnInitializeUpdate()
        {
            if (!EditorPrefs.GetBool(KEY_AUTO_CHECK_UPDATES, true)) return;

            string strLastCheck = EditorPrefs.GetString(
                KEY_LAST_CHECK_DATE,
                DateTime.MinValue.ToString()
            );

            DateTime lastCheck;
            if (!DateTime.TryParse(strLastCheck, out lastCheck))
            {
                lastCheck = DateTime.MinValue;
            }

            TimeSpan timeSpan = DateTime.Now - lastCheck;
            if (timeSpan.TotalDays >= 1)
            {
                SaveCheckTime();
                UpdateHttpRequest.Request(GAMECREATOR_BUNDLE, OnRetrieveUpdate);
            }
        }

        // CALLBACK METHODS: ----------------------------------------------------------------------

        public static void OnRetrieveUpdate(bool isError, UpdateHttpRequest.OutputData data)
        {
            CHECKING_UPDATES = false;
            if (isError || data == null || data.error) return;

            EditorPrefs.SetString(KEY_UPDATE_CACHE, EditorJsonUtility.ToJson(data.data));
            if (GameCreatorUpdateWindow.WINDOW != null) GameCreatorUpdateWindow.WINDOW.Refresh();

            Version skipVersion = new Version(EditorPrefs.GetString(KEY_SKIP_PACKAGE_VERSION, "0.0.0"));
            if (data.data.version.Equals(skipVersion) && GameCreatorUpdateWindow.WINDOW == null) return;

            if (data.data.version.HigherThan(Config.GetCurrent().version))
            {
                string message = string.Empty;
                switch (data.data.type)
                {
                    case "patch": message = PATCH; break;
                    case "release": message = RELEASE; break;
                    case "alpha": message = ALPHA; break;
                }

                int option = EditorUtility.DisplayDialogComplex(
                    "New Game Creator version available.",
                    message,
                    "Download " + data.data.version,
                    "Skip version",
                    "Remind me tomorrow"
                );

                switch (option)
                {
                    case 0: Application.OpenURL(URL_DOWNLOAD); break;
                    case 1: SkipVersion(data.data.version);  break;
                    case 2: SaveCheckTime(); break;
                }
            }
            else if (GameCreatorUpdateWindow.WINDOW != null)
            {
                EditorUtility.DisplayDialog(
                    "Game Creator is up to date.",
                    string.Format(
                        "You're currently using {0}, which is the latest version.",
                        Config.GetCurrent().version
                    ),
                    "Ok"
                );
            }
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private static void SkipVersion(Version version)
        {
            EditorPrefs.SetString(KEY_SKIP_PACKAGE_VERSION, version.ToString());
        }

        private static void SaveCheckTime()
        {
            EditorPrefs.SetString(KEY_LAST_CHECK_DATE, DateTime.Now.ToString());
        }
    }
}