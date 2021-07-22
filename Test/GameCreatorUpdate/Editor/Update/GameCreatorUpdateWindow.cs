namespace GameCreator.Update
{
    using System;
    using System.IO;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    public class GameCreatorUpdateWindow : EditorWindow
    {
        private const float WINDOW_W = 500f;
        private const float WINDOW_H = 400f;

        private const string TITLE = "Game Creator Updates";
        public static GameCreatorUpdateWindow WINDOW { get; private set; }

        private static readonly GUIContent GC_UPDATES = new GUIContent(" Check for Updates");
        private static readonly GUIContent GC_CHECKNOW = new GUIContent("Check Now");
        private static readonly GUIContent GC_CHECKING = new GUIContent("Checking...");
        private static readonly GUIContent GC_FORCEINS = new GUIContent("Force Install");

        private const string VERSION = "Current version: {0}";
        private const string UI_DOWNLOAD = "Download {0}";
        private const string UI_TITLE = "Game Creator {0} - {1}";
        private const string UI_CHANGELOG = "Changelog:";

        // PROPERTIES: ----------------------------------------------------------------------------

        private bool stylesInitialized = false;
        private GUIStyle styleMargins;
        private GUIStyle styleLabelVersion;
        private GUIStyle styleButtonLarge;

        private bool autoCheckUpdates = false;

        private UpdateHttpRequest.OutputData.Data updateData = null;
        private string[] updateDataChangelog = new string[0];

        // INITIALIZERS: --------------------------------------------------------------------------

        [MenuItem("Game Creator/Check for Updates...", priority = 100)]
        private static void OpenWindow()
        {
            WINDOW = EditorWindow.GetWindowWithRect<GameCreatorUpdateWindow>(
                new Rect(0f, 0f, WINDOW_W, WINDOW_H),
                true, TITLE, true
            );

            WINDOW.Show();
        }

        private void OnEnable()
        {
            this.Refresh();
        }

        public void Refresh()
        {
            WINDOW = this;
            this.autoCheckUpdates = EditorPrefs.GetBool(
                GameCreatorUpdate.KEY_AUTO_CHECK_UPDATES,
                true
            );

            string updateDataJson = EditorPrefs.GetString(GameCreatorUpdate.KEY_UPDATE_CACHE, "");
            this.updateData = null;
            this.updateDataChangelog = new string[0];

            if (!string.IsNullOrEmpty(updateDataJson))
            {
                this.updateData = JsonUtility.FromJson(
                    updateDataJson,
                    typeof(UpdateHttpRequest.OutputData.Data)
                ) as UpdateHttpRequest.OutputData.Data;

                if (this.updateData != null)
                {
                    string date = this.updateData.date;
                    this.updateData.date = DateTime.Parse(date).ToString("D");

                    this.updateDataChangelog = this.updateData.changelog.Split('\n');
                }
            }

            this.Repaint();
        }

        // PAINT METHODS: -------------------------------------------------------------------------

        private void OnGUI()
        {
            this.InitializeStyles();
            this.PaintHead();
            this.PaintBody();
        }

        private void PaintHead()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            Rect rectCheckUpdates = GUILayoutUtility.GetRect(GC_UPDATES, EditorStyles.toolbarButton);
            bool checkUpdates = GUI.Toggle(
                rectCheckUpdates,
                this.autoCheckUpdates,
                GC_UPDATES,
                EditorStyles.toolbarButton
            );

            if (checkUpdates != this.autoCheckUpdates)
            {
                this.autoCheckUpdates = checkUpdates;
                EditorPrefs.SetBool(
                    GameCreatorUpdate.KEY_AUTO_CHECK_UPDATES,
                    this.autoCheckUpdates
                );
            }

            EditorGUI.BeginDisabledGroup(GameCreatorUpdate.CHECKING_UPDATES);

            bool onClickCheckNow = GUILayout.Button(
                GameCreatorUpdate.CHECKING_UPDATES ? GC_CHECKING : GC_CHECKNOW,
                EditorStyles.toolbarButton
            );

            if (onClickCheckNow && !GameCreatorUpdate.CHECKING_UPDATES)
            {
                UpdateHttpRequest.Request(
                    GameCreatorUpdate.GAMECREATOR_BUNDLE,
                    GameCreatorUpdate.OnRetrieveUpdate
                );
            }

            EditorGUI.EndDisabledGroup();
            GUILayout.FlexibleSpace();

            EditorGUILayout.LabelField(
                string.Format(VERSION, Config.GetCurrent().version),
                this.styleLabelVersion
            );
                
            if (GUILayout.Button(GC_FORCEINS, EditorStyles.toolbarButton))
            {
                this.Close();
                GameCreatorInstallWindow.OpenWindow();
            }

            EditorGUILayout.EndHorizontal();
        }

        private Vector2 scroll = Vector2.zero;

        private void PaintBody()
        {
            this.scroll = EditorGUILayout.BeginScrollView(this.scroll, this.styleMargins);
            GameCreatorInstall.Requirement requirement = GameCreatorInstall.MeetsUnityRequirements();

            if (!requirement.success)
            {
                EditorGUILayout.HelpBox(
                    requirement.message,
                    MessageType.Error
                );
            }
            else if (!string.IsNullOrEmpty(requirement.message))
            {
                EditorGUILayout.HelpBox(
                    requirement.message,
                    MessageType.Warning
                );
            }

            if (this.updateData == null)
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField(
                    "No information available.",
                    EditorStyles.centeredGreyMiniLabel
                );
                GUILayout.FlexibleSpace();
            }
            else
            {
                this.PaintBodyUpdateAvailable();
            }

            EditorGUILayout.EndScrollView();
        }

        private void PaintBodyUpdateAvailable()
        {
            if (this.updateData.version.HigherThan(Config.GetCurrent().version))
            {
                EditorGUILayout.BeginHorizontal();

                string download = string.Format(UI_DOWNLOAD, this.updateData.version);
                if (GUILayout.Button(download, this.styleButtonLarge))
                {
                    Application.OpenURL(GameCreatorUpdate.URL_DOWNLOAD);
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }

            EditorGUILayout.LabelField(
                string.Format(UI_TITLE, this.updateData.version, this.updateData.type.ToUpper()),
                EditorStyles.boldLabel
            );
                
            EditorGUILayout.LabelField(this.updateData.date);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(UI_CHANGELOG, EditorStyles.miniBoldLabel);
            EditorGUILayout.Space();

            for (int i = 0; i < this.updateDataChangelog.Length; ++i)
            {
                EditorGUILayout.LabelField(this.updateDataChangelog[i]);
            }

            EditorGUILayout.Space();
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void InitializeStyles()
        {
            if (this.stylesInitialized) return;

            this.styleMargins = new GUIStyle();
            this.styleMargins.padding = new RectOffset(10, 10, 10, 10);

            this.styleButtonLarge = new GUIStyle(GUI.skin.GetStyle("Button"));
            this.styleButtonLarge.padding = new RectOffset(20, 20, 0, 0);
            this.styleButtonLarge.fixedHeight = 30;

            this.styleLabelVersion = new GUIStyle(EditorStyles.miniBoldLabel);
            this.styleLabelVersion.alignment = TextAnchor.UpperRight;

            this.stylesInitialized = true;
        }
    }
}