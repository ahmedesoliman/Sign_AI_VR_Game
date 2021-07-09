namespace GameCreator.Core
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using UnityEngine.Events;

    public class PreferencesWindow : EditorWindow
    {
        private const string WIN_TITLE = "Game Creator Preferences";
        private const string KEY_SIDEBAR_INDEX = "gamecreator-preferences-index";

        public const float WIN_WIDTH = 800.0f;
        public const float WIN_HEIGHT = 700.0f;
        public const float SIDEBAR_WIDTH = 150.0f;

        private static PreferencesWindow Instance;

        private class DatabaseInfo : IComparable<DatabaseInfo>
        {
            public string name;
            public IDatabase data;
            public IDatabaseEditor dataEditor;

            public DatabaseInfo(IDatabase data)
            {
                this.data = data;
                this.dataEditor = Editor.CreateEditor(this.data) as IDatabaseEditor;
                this.name = this.dataEditor.GetName();
            }

            public int CompareTo(DatabaseInfo value)
            {
                int valueA = this.dataEditor.GetPanelWeight();
                int valueB = value.dataEditor.GetPanelWeight();
                return valueA.CompareTo(valueB);
            }
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        private Vector2 scrollSidebar = Vector2.zero;
        private Vector2 scrollContent = Vector2.zero;
        private int sidebarIndex = 0;

        private bool initStyles = false;
        private GUIStyle styleSidebar;

        private static List<DatabaseInfo> DATABASES = new List<DatabaseInfo>();

        // INITIALIZE METHODS: --------------------------------------------------------------------

        private void OnEnable()
        {
            this.initStyles = false;
            this.ChangeSidebarIndex(EditorPrefs.GetInt(KEY_SIDEBAR_INDEX, 0));
        }

        // GUI METHODS: ---------------------------------------------------------------------------

        private void OnGUI()
        {
            if (PreferencesWindow.Instance == null)
            {
                PreferencesWindow.OpenWindow();
            }

            if (!this.initStyles) this.InitializeStyles();

            int currentSidebarIndex = this.sidebarIndex;
            if (currentSidebarIndex < 0)
            {
                currentSidebarIndex = 0;
                this.ChangeSidebarIndex(currentSidebarIndex);
            }
            else if (currentSidebarIndex >= DATABASES.Count)
            {
                currentSidebarIndex = DATABASES.Count - 1;
                this.ChangeSidebarIndex(currentSidebarIndex);
            }

            EditorGUILayout.BeginHorizontal();

            this.PaintSidebar(currentSidebarIndex);

            EditorGUILayout.BeginVertical();
            this.PaintToolbar(currentSidebarIndex);
            this.PaintContent(currentSidebarIndex);
            EditorGUILayout.EndVertical();

            this.Repaint();
            EditorGUILayout.EndHorizontal();
        }

        private void PaintSidebar(int currentSidebarIndex)
        {
            this.scrollSidebar = EditorGUILayout.BeginScrollView(
                this.scrollSidebar,
                this.styleSidebar,
                GUILayout.MinWidth(SIDEBAR_WIDTH),
                GUILayout.MaxWidth(SIDEBAR_WIDTH),
                GUILayout.ExpandHeight(true)
            );

            for (int i = DATABASES.Count - 1; i >= 0; --i)
            {
                if (DATABASES[i].data == null) DATABASES.RemoveAt(i);
            }

            for (int i = 0; i < DATABASES.Count; ++i)
            {
                Rect itemRect = GUILayoutUtility.GetRect(GUIContent.none, CoreGUIStyles.GetItemPreferencesSidebar());

                if (UnityEngine.Event.current.type == EventType.MouseDown &&
                    itemRect.Contains(UnityEngine.Event.current.mousePosition))
                {
                    this.ChangeSidebarIndex(i);
                }

                bool isActive = (currentSidebarIndex == i);

                if (UnityEngine.Event.current.type == EventType.Repaint)
                {
                    string text = DATABASES[i].name;
                    CoreGUIStyles.GetItemPreferencesSidebar().Draw(itemRect, text, isActive, isActive, false, false);
                }
            }

            EditorGUILayout.EndScrollView();

            Rect borderRect = GUILayoutUtility.GetRect(1f, 1f, GUILayout.ExpandHeight(true), GUILayout.Width(1f));
            EditorGUI.DrawTextureAlpha(borderRect, Texture2D.blackTexture);
        }

        private void LoadDatabases()
        {
            List<IDatabase> databases = GameCreatorUtilities.FindAssetsByType<IDatabase>();
            int databasesCount = databases.Count;

            DATABASES = new List<DatabaseInfo>();
            for (int i = 0; i < databasesCount; ++i)
            {
                DATABASES.Add(new DatabaseInfo(databases[i]));
            }

            DATABASES.Sort((DatabaseInfo x, DatabaseInfo y) =>
            {
                int valueX = x.data.GetSidebarPriority();
                int valueY = y.data.GetSidebarPriority();
                return valueX.CompareTo(valueY);
            });
        }

        private void PaintToolbar(int currentSidebarIndex)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.FlexibleSpace();

            currentSidebarIndex = Mathf.Clamp(currentSidebarIndex, 0, DATABASES.Count - 1);
            if (DATABASES[currentSidebarIndex].dataEditor.CanBeDecoupled())
            {
                if (GUILayout.Button("Detach", EditorStyles.toolbarButton))
                {
                    PreferencesDetachWindow.Create(DATABASES[currentSidebarIndex].dataEditor);
                }
            }

            if (GUILayout.Button("Documentation", EditorStyles.toolbarButton))
            {
                Application.OpenURL(DATABASES[currentSidebarIndex].dataEditor.GetDocumentationURL());
            }

            EditorGUILayout.EndHorizontal();
        }

        private void PaintContent(int currentSidebarIndex)
        {
            if (DATABASES.Count == 0) return;
            this.scrollContent = EditorGUILayout.BeginScrollView(
                this.scrollContent,
                GUILayout.ExpandWidth(true),
                GUILayout.ExpandHeight(true)
            );

            EditorGUILayout.Space();
            DATABASES[currentSidebarIndex].dataEditor.OnPreferencesWindowGUI();
            EditorGUILayout.Space();

            EditorGUILayout.EndScrollView();
        }

        private void InitializeStyles()
        {
            Texture2D texture = new Texture2D(1, 1);
            if (EditorGUIUtility.isProSkin) texture.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.35f));
            else texture.SetPixel(0, 0, new Color(256f, 256f, 256f, 0.5f));

			texture.alphaIsTransparency = true;
			texture.Apply();

			this.styleSidebar = new GUIStyle();
			this.styleSidebar.normal.background = texture;
			this.styleSidebar.margin = new RectOffset(0,0,0,0);

			this.initStyles = true;
		}

		// OPEN WINDOW SHORTCUT: ------------------------------------------------------------------

		[MenuItem("Game Creator/Preferences %&k")]
		public static PreferencesWindow OpenWindow()
		{
			PreferencesWindow window = EditorWindow.GetWindow<PreferencesWindow>(
				true, 
				PreferencesWindow.GetWindowTitle(), 
				true
			);

			PreferencesWindow.Instance = window;
            window.LoadDatabases();
			window.Show();
			return window;
		}

	    [MenuItem("Game Creator/Tools/Documentation...")]
	    public static void OpenDocumentation()
	    {
		    Application.OpenURL("https://docs.gamecreator.io");
	    }

		public static void CloseWindow()
		{
			if (PreferencesWindow.Instance == null) return;
			PreferencesWindow.Instance.Close();
		}

		public static void OpenWindowTab(string tabName)
		{
			PreferencesWindow window = PreferencesWindow.OpenWindow();

			tabName = tabName.ToLower();
            for (int i = 0; i < DATABASES.Count; ++i)
			{
                if (DATABASES[i].name.ToLower() == tabName) 
				{
					window.sidebarIndex = i;
					break;
				}
			}
		}

		private void ChangeSidebarIndex(int nextIndex)
		{
			this.sidebarIndex = nextIndex;
			EditorPrefs.SetInt(KEY_SIDEBAR_INDEX, this.sidebarIndex);

			string windowName = PreferencesWindow.GetWindowTitle();
			this.titleContent = new GUIContent(windowName);
		}

		private static string GetWindowTitle()
		{
            return PreferencesWindow.WIN_TITLE;
		}

		// PUBLIC METHODS: ------------------------------------------------------------------------

		public static void SetSidebarIndex(int index)
		{
			EditorPrefs.SetInt(KEY_SIDEBAR_INDEX, index);
            if (PreferencesWindow.Instance != null)
            {
                PreferencesWindow.Instance.ChangeSidebarIndex(index);
            }
        }
	}
}