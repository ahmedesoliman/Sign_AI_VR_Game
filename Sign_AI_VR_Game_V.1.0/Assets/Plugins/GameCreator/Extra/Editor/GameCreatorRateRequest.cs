namespace GameCreator.Core.Rate
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    public class GameCreatorRateRequest : EditorWindow
    {
        private const string KEY_PLAY_TIMES = "gc-unity-editor-open-count";
        private const string KEY_FIRST_TIME = "gc-unity-editor-first-time";
        private const string KEY_ALREADY_SN = "gc-unity-editor-already-se";

        private const string PATH_LOGO = "Assets/Plugins/GameCreator/Extra/Icons/Rate/Logo@{0}x.png";
        private const string PATH_REVIEW = "Assets/Plugins/GameCreator/Extra/Icons/Rate/IconReview.png";

        private const string MSG_TITLE = "Thank you for using Game Creator!";
        private const string MSG_TEXT1 = "We hate to disturb you like this, but consider " +
            "leaving a review in the Asset Store.\n\nSpreading the word allow us to " +
            "keep developing Game Creator and adding exciting new features.";

        private static readonly GUIContent MSG_TEXT2 = new GUIContent(
            "This will only appear once and we will not bother you again. Happy game making!"
        );

        private static readonly Color COLOR_FOOTER = new Color(0, 0, 0, 0.1f);

        private static GameCreatorRateRequest Instance;
        private static readonly Vector2 WIN_SIZE = new Vector2(300, 400);

        private const int SPACE = 20;
        private const int LOGO_SIZE = 80;

        // PROPERTIES: ----------------------------------------------------------------------------

        private bool showFooter = false;
        private bool stylesInitialized = false;

        private GUIStyle styleTitle;
        private GUIStyle styleText;
        private GUIStyle styleFooter;
        private GUIStyle styleReview;
        private Texture2D textureLogo;
        private GUIContent gcReview;

        // INITIALIZERS: --------------------------------------------------------------------------

        [InitializeOnLoadMethod]
        private static void OnInitialize()
        {
            if (EditorPrefs.GetInt(KEY_ALREADY_SN, 0) != 0) return;
            EditorApplication.playModeStateChanged += OnPlaymodeChange;
        }

        private static void OnPlaymodeChange(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.EnteredEditMode) return;

            int count = EditorPrefs.GetInt(KEY_PLAY_TIMES, 0);
            EditorPrefs.SetInt(KEY_PLAY_TIMES, count + 1);

            string strDate = EditorPrefs.GetString(KEY_FIRST_TIME, string.Empty);
            if (string.IsNullOrEmpty(strDate))
            {
                strDate = DateTime.Now.ToString("d");
                EditorPrefs.SetString(KEY_FIRST_TIME, strDate);
            }

            DateTime date = DateTime.Parse(strDate);
            if ((DateTime.Now - date).Days >= 3 && count >= 10)
            {
                OpenWindow(true);
                EditorPrefs.SetInt(KEY_ALREADY_SN, 1);
            }
        }

        private void InitializeStyles()
        {
            this.styleTitle = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true
            };

            this.styleText = new GUIStyle(EditorStyles.label)
            {
                padding = new RectOffset(SPACE, SPACE, 0, 0),
                fontStyle = FontStyle.Normal,
                alignment = TextAnchor.MiddleCenter,
                richText = true,
                wordWrap = true,
            };

            this.styleFooter = new GUIStyle(this.styleText)
            {
                margin = new RectOffset(0,0,0,0),
                padding = new RectOffset(8,8,8,8),
                alignment = TextAnchor.MiddleLeft,
                fontSize = EditorStyles.centeredGreyMiniLabel.fontSize,
            };

            this.styleReview = new GUIStyle(GUI.skin.button)
            {
                padding = new RectOffset(0, 0, SPACE/2, SPACE/2),
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Normal,
                richText = true,
            };

            this.textureLogo = AssetDatabase.LoadAssetAtPath<Texture2D>(
                string.Format(PATH_LOGO, (EditorGUIUtility.pixelsPerPoint > 1f ? 2 : 1)
            ));

            this.gcReview = new GUIContent(
                " Review in the <b>Unity Asset Store</b>",
                AssetDatabase.LoadAssetAtPath<Texture2D>(PATH_REVIEW)
            );

            this.stylesInitialized = true;
        }

        // PAINT METHODS: -------------------------------------------------------------------------

        [MenuItem("Game Creator/Tools/Review...")]
        public static void OpenWindow()
        {
            OpenWindow(false);
        }

        public static void OpenWindow(bool showFooter)
        {
            Instance = EditorWindow.GetWindow<GameCreatorRateRequest>(true, string.Empty, true);

            Instance.showFooter = showFooter;
            Instance.maxSize = WIN_SIZE;
            Instance.minSize = WIN_SIZE;
            Instance.ShowTab();
        }

        private void OnGUI()
        {
            if (!this.stylesInitialized)
            {
                this.InitializeStyles();
            }

            GUILayout.Space(SPACE);

            Rect rectLogoTotal = GUILayoutUtility.GetRect(WIN_SIZE.x, LOGO_SIZE);
            Rect rectLogo = new Rect(
                rectLogoTotal.x + (rectLogoTotal.width/2f - LOGO_SIZE/2f),
                rectLogoTotal.y,
                LOGO_SIZE,
                LOGO_SIZE
            );

            GUI.DrawTexture(rectLogo, this.textureLogo);
            GUILayout.Space(SPACE);

            Rect rectTitle = GUILayoutUtility.GetRect(WIN_SIZE.x, 20);
            EditorGUI.LabelField(rectTitle, MSG_TITLE, this.styleTitle);

            GUILayout.Space(SPACE);
            EditorGUILayout.LabelField(MSG_TEXT1, this.styleText);

            GUILayout.Space(SPACE);
            if (GUILayout.Button(this.gcReview, this.styleReview, GUILayout.Height(40)))
            {
                Application.OpenURL("https://gamecreator.page.link/review");
                this.Close();
            }

            if (!this.showFooter) return;
            GUILayout.FlexibleSpace();

            Rect rectFooter = GUILayoutUtility.GetRect(MSG_TEXT2, this.styleFooter);

            EditorGUI.DrawRect(rectFooter, COLOR_FOOTER);
            EditorGUI.LabelField(rectFooter, MSG_TEXT2, this.styleFooter);
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------
    }
}