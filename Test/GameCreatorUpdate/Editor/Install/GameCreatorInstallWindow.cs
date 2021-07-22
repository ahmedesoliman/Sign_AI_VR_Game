namespace GameCreator.Update
{
    using System;
    using System.IO;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    public class GameCreatorInstallWindow : EditorWindow
    {
        private const float WINDOW_W = 300f;
        private const float WINDOW_H = 250f;
        private const float WINDOW_P = 5f;

        private const float LOGO_SIZE = 80f;
        private const string LOGO_PATH = "Assets/Plugins/GameCreatorUpdate/Editor/Icons/Logo@{0}x.png";

        private const string TITLE = "Game Creator Install";
        public static GameCreatorInstallWindow WINDOW { get; private set; }

        private const string MSG_BUTTON = "Install Game Creator {0}";
        private const string MSG_INFO = "Current Game Creator version: {0}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private bool stylesInitialized = false;
        private Texture2D textureLogo;

        // INITIALIZERS: --------------------------------------------------------------------------

        [MenuItem("Game Creator/Reinstall Game Creator...", priority = 101)]
        public static void OpenWindow()
        {
            WINDOW = EditorWindow.GetWindowWithRect<GameCreatorInstallWindow>(
                new Rect(0f, 0f, WINDOW_W, WINDOW_H),
                true, TITLE, true
            );

            WINDOW.Show();
        }

        private void OnEnable()
        {
            WINDOW = this;
        }

        // PAINT METHODS: -------------------------------------------------------------------------

        private void OnGUI()
        {
            this.InitializeStyles();
            this.PaintGUI();
        }

        private void PaintGUI()
        {
            GUILayout.Space(WINDOW_P);
            GUILayout.FlexibleSpace();

            Rect rectLogoTotal = GUILayoutUtility.GetRect(WINDOW_W, LOGO_SIZE);
            Rect rectLogo = new Rect(
                rectLogoTotal.x + (rectLogoTotal.width / 2f - LOGO_SIZE / 2f),
                rectLogoTotal.y,
                LOGO_SIZE,
                LOGO_SIZE
            );

            GUI.DrawTexture(rectLogo, this.textureLogo);
            GUILayout.Space(WINDOW_P);
            GUILayout.FlexibleSpace();
            GUILayout.Space(WINDOW_P);

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

            Rect rectButton = GUILayoutUtility.GetRect(WINDOW_W, 50f);
            rectButton.x += WINDOW_P;
            rectButton.width -= (WINDOW_P * 2f);

            string contButton = string.Format(MSG_BUTTON, Config.GetUpdate().version);

            EditorGUI.BeginDisabledGroup(!requirement.success);
            if (GUI.Button(rectButton, contButton))
            {
                this.Close();
                GameCreatorInstall.InstallUpdate();
            }
            EditorGUI.EndDisabledGroup();
            GUILayout.Space(WINDOW_P);

            EditorGUILayout.HelpBox(
                string.Format(MSG_INFO, Config.GetCurrent().version),
                MessageType.None
            );

            GUILayout.Space(WINDOW_P);
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void InitializeStyles()
        {
            if (this.stylesInitialized) return;

            this.textureLogo = AssetDatabase.LoadAssetAtPath<Texture2D>(
                string.Format(LOGO_PATH, (EditorGUIUtility.pixelsPerPoint > 1f ? 2 : 1)
            ));
        }
    }
}