namespace GameCreator.Core
{
	using System;
	using System.IO;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using UnityEditor.AnimatedValues;
	using System.Linq;
	using System.Reflection;
    using GameCreator.ModuleManager;

	[CustomEditor(typeof(DatabaseQuickstart))]
	public class DatabaseQuickstartEditor : IDatabaseEditor
	{
		private class PageData
		{
			private const string TEXTURE_HEADER_PATH = "Assets/Plugins/GameCreator/Extra/Icons/Quickstart/";

			public Action<Rect, Texture2D> paint;
			public Texture2D header;

			public PageData(Action<Rect, Texture2D> paint, string headerName)
			{
				string headerPath = Path.Combine(TEXTURE_HEADER_PATH, headerName);
				this.header = AssetDatabase.LoadAssetAtPath<Texture2D>(headerPath);
				this.paint = paint;
			}
		}

		// PROPERTIES: -------------------------------------------------------------------------------------------------

		private const float CONTROL_MARGIN_BOTTOM = 20.0f;
		private const float CONTROL_DOT_WIDTH = 20f;
		private const float CONTROL_BTN_WIDTH = 80.0f;
		private const float HEADER_IMAGES_AR = 900f/500f;

		private AnimFloat animPagesIndex;
		private PageData[] pages;

		private GUIStyle titleStyle;
		private GUIStyle contentStyle;

		private float availableWidth = 0.0f;
		private float availableHeight = 0.0f;

		// INITIALIZE: -------------------------------------------------------------------------------------------------

		private void OnEnable()
		{
            if (target == null || serializedObject == null) return;
			this.animPagesIndex = new AnimFloat(0.0f, () => { this.Repaint(); });
			this.pages = new PageData[]
			{
				new PageData(this.OnPaintWelcome, "welcome.png"),
				new PageData(this.OnPaintTutorials, "tutorials.png"),
				new PageData(this.OnPaintDocumentation, "documentation.png"),
				new PageData(this.OnPaintExamples, "examples.png"),
				new PageData(this.OnPaintModules, "modules.png"),
				new PageData(this.OnPaintStore, "hub.png"),
                new PageData(this.OnPaintDiscord, "discord.png"),
			};
		}

		private void InitializeStyles()
		{
			if (this.titleStyle == null)
			{
				this.titleStyle = new GUIStyle(EditorStyles.boldLabel);
				this.titleStyle.fontSize = 13;
				this.titleStyle.fontStyle = FontStyle.Bold;
				this.titleStyle.alignment = TextAnchor.MiddleCenter;
			}

			if (this.contentStyle == null)
			{
				this.contentStyle = new GUIStyle(EditorStyles.helpBox);
				this.contentStyle.padding = new RectOffset(10,10,10,10);
				this.contentStyle.alignment = TextAnchor.MiddleCenter;
				this.contentStyle.richText = true;
				this.contentStyle.wordWrap = true;
			}
		}

		// OVERRIDE METHODS: -------------------------------------------------------------------------------------------

		public override string GetName ()
		{
			return "Quickstart";
		}

		public override int GetPanelWeight ()
		{
			return 99;
		}

		// GUI METHODS: ------------------------------------------------------------------------------------------------

		public override void OnInspectorGUI()
		{
			EditorGUILayout.HelpBox("This component is only accessable through the Preferences Panel", MessageType.Info);
			if (GUILayout.Button("Open Preferences Window"))
			{
				PreferencesWindow.OpenWindow();
			}
		}

		public override void OnPreferencesWindowGUI()
		{
			this.serializedObject.Update();

			this.InitializeStyles();
			this.PaintPagesView();

			this.serializedObject.ApplyModifiedProperties();
		}

		private void PaintPagesView()
		{
			Rect availableSpace = GUILayoutUtility.GetRect(
				GUIContent.none, 
				GUIStyle.none, 
				GUILayout.ExpandWidth(true), 
				GUILayout.ExpandHeight(true)
			);

			float areaWidth = this.availableWidth;
			float areaHeight = this.availableHeight;

			if (UnityEngine.Event.current.type == EventType.Repaint)
			{
                this.availableWidth = availableSpace.width;
				this.availableHeight = availableSpace.height;
            }


			if (areaWidth <= Mathf.Epsilon || areaHeight <= Mathf.Epsilon) return;

			GUILayout.BeginArea(new Rect(
				this.animPagesIndex.value * -areaWidth, 
				0, 
				areaWidth * this.pages.Length, 
				areaHeight
			));

			for (int i = 0; i < this.pages.Length; ++i)
			{
				Rect bounds = new Rect(
					(float)i * areaWidth,
					0.0f, 
					areaWidth,
					areaHeight
				);

				this.pages[i].paint(bounds, this.pages[i].header);
			}

			GUILayout.EndArea();
			this.PaintControls();
		}

		private void PaintControls()
		{
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            EditorGUI.BeginDisabledGroup(Mathf.Approximately(this.animPagesIndex.target, 0.0f));
            if (GUILayout.Button("Previous", CoreGUIStyles.GetButtonLeft()))
            {
                float targ = this.animPagesIndex.target - 1.0f;
                if (targ < 0.0f) targ = 0.0f;
                this.animPagesIndex.target = targ;

            }
            EditorGUI.EndDisabledGroup();

            for (int i = 0; i < this.pages.Length; ++i)
            {
                GUIStyle dotStyle = (Mathf.Approximately(this.animPagesIndex.target, (float)i)
                    ? CoreGUIStyles.GetToggleButtonMidOn()
                    : CoreGUIStyles.GetToggleButtonMidOff()
                );

                if (GUILayout.Button((i + 1).ToString(), dotStyle))
                {
                    this.animPagesIndex.target = (float)i;
                }
            }

            EditorGUI.BeginDisabledGroup(Mathf.Approximately(this.animPagesIndex.target, this.pages.Length - 1.0f));
            if (GUILayout.Button("Next", CoreGUIStyles.GetButtonRight()))
            {
                float targ = this.animPagesIndex.target + 1.0f;
                if (targ > this.pages.Length - 1.0f) targ = this.pages.Length - 1.0f;
                this.animPagesIndex.target = targ;
            }
            EditorGUI.EndDisabledGroup();

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
		}

		// PAGES METHODS: ----------------------------------------------------------------------------------------------

		private Rect OnPaintPage(Rect bounds, string title, Texture2D header, float heightReserve = 80f)
		{
			Rect titleRect = GUILayoutUtility.GetRect(bounds.width, 50f);
			EditorGUI.LabelField(titleRect, title, this.titleStyle);


			Rect headerRect = GUILayoutUtility.GetRect(bounds.width, bounds.width/HEADER_IMAGES_AR);
			EditorGUI.DrawPreviewTexture(headerRect, header);

			Rect contentRect = GUILayoutUtility.GetRect(bounds.width, heightReserve);
			return new Rect(
				contentRect.x + 20f,
				contentRect.y + 20f,
				contentRect.width - 40f,
				contentRect.height - 20f
			);
		}

		private void OnPaintWelcome(Rect bounds, Texture2D header)
		{
			GUILayout.BeginArea(bounds);

			Rect contentRect = this.OnPaintPage(bounds, "WELCOME TO GAME CREATOR", header, 80f);

			string content = "Follow these simple steps and become a pro with <b>Game Creator</b> in less than 15 minutes";
			EditorGUI.LabelField(contentRect, content, this.contentStyle);
			GUILayout.EndArea();
		}

		private void OnPaintTutorials(Rect bounds, Texture2D header)
		{
			GUILayout.BeginArea(bounds);

			string videoTutorialsURL = "https://www.youtube.com/watch?v=IG8GXAAih2Q&list=PL7FyK0gfdpCbxMrWIV9B2xQiExkiZbpa5";
			string content = "Watch the <b>Getting Started Video Tutorials</b>";

			Rect contentRect = this.OnPaintPage(bounds, "GET STARTED IN 15 MINUTES", header, 80f);
			EditorGUI.LabelField(contentRect, content, contentStyle);

			Rect btnRect = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.button);
			btnRect = new Rect(btnRect.x + 15f, btnRect.y, btnRect.width - 30f, btnRect.height);
			if (GUI.Button(btnRect, "Watch playlist"))
			{
				Application.OpenURL(videoTutorialsURL);
			}

			GUILayout.EndArea();
		}

		private void OnPaintDocumentation(Rect bounds, Texture2D header)
		{
			GUILayout.BeginArea(bounds);

			string documentationURL = "https://docs.gamecreator.io";
			string content = "Take a look at our beautifully hand-crafted <b>Documentation</b>";

			Rect contentRect = this.OnPaintPage(bounds, "DOCUMENTATION", header, 80f);
			EditorGUI.LabelField(contentRect, content, contentStyle);

			Rect btnRect = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.button);
			btnRect = new Rect(btnRect.x + 15f, btnRect.y, btnRect.width - 30f, btnRect.height);
			if (GUI.Button(btnRect, "docs.gamecreator.io"))
			{
				Application.OpenURL(documentationURL);
			}

			GUILayout.EndArea();
		}

		private void OnPaintExamples(Rect bounds, Texture2D header)
		{
			GUILayout.BeginArea(bounds);

			string content = "Learn from the <b>Example Scenes</b>";

			Rect contentRect = this.OnPaintPage(bounds, "EXAMPLE SCENES", header, 80f);
			EditorGUI.LabelField(contentRect, content, contentStyle);

			Rect btnRect = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.button);
			btnRect = new Rect(btnRect.x + 15f, btnRect.y, btnRect.width - 30f, btnRect.height);
			if (GUI.Button(btnRect, "See Example Scenes"))
			{
				string scenePath = "Assets/Plugins/GameCreator/Examples/Scenes/Example-Hub.unity";
                SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
                if (sceneAsset != null)
                {
                    Selection.activeObject = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
                    EditorGUIUtility.PingObject(Selection.activeObject);
                }
                else
                {
                    ModuleManagerWindow.OpenModuleManager();
                }
			}

			GUILayout.EndArea();
		}

		private void OnPaintModules(Rect bounds, Texture2D header)
		{
			GUILayout.BeginArea(bounds);

			string content = "Check out the <b>Module Manager</b>";
			Rect contentRect = this.OnPaintPage(bounds, "MODULE MANAGER", header, 80f);
			EditorGUI.LabelField(contentRect, content, contentStyle);

			Rect btnRect = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.button);
			btnRect = new Rect(btnRect.x + 15f, btnRect.y, btnRect.width - 30f, btnRect.height);
			if (GUI.Button(btnRect, "Module Manager"))
			{
                ModuleManagerWindow.OpenModuleManager();
			}

			GUILayout.EndArea();
		}

		private void OnPaintStore(Rect bounds, Texture2D header)
		{
			GUILayout.BeginArea(bounds);

			string storeURL = "https://hub.gamecreator.io";
			string content = "Visit <b>Game Creator Hub</b>";

			Rect contentRect = this.OnPaintPage(bounds, "GAME CREATOR HUB", header, 80f);
			EditorGUI.LabelField(contentRect, content, contentStyle);

			Rect btnRect = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.button);
			btnRect = new Rect(btnRect.x + 15f, btnRect.y, btnRect.width - 30f, btnRect.height);
			if (GUI.Button(btnRect, "hub.gamecreator.io"))
			{
				Application.OpenURL(storeURL);
			}

			GUILayout.EndArea();
		}

        private void OnPaintDiscord(Rect bounds, Texture2D header)
        {
            GUILayout.BeginArea(bounds);

            string discordURL = "https://gamecreator.page.link/discord";
            string content = "Be part of a friendly community of game developers";

            Rect contentRect = this.OnPaintPage(bounds, "JOIN OUR DISCORD SERVER", header, 80f);
            EditorGUI.LabelField(contentRect, content, contentStyle);

            Rect btnRect = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.button);
            btnRect = new Rect(btnRect.x + 15f, btnRect.y, btnRect.width - 30f, btnRect.height);
            if (GUI.Button(btnRect, "Join Game Creator Discord"))
            {
                Application.OpenURL(discordURL);
            }

            GUILayout.EndArea();
        }
	}

	// ON INITIALIZE SHOW PREFERENCES: ---------------------------------------------------------------------------------

	[InitializeOnLoad]
	public class DatabaseQuickstartEditorOnLoad 
	{
		private const string KEY_PREFERENCES_STARTUP = "show-quickstart-preferences-on-startup";

		static DatabaseQuickstartEditorOnLoad()
		{
			EditorApplication.update += DatabaseQuickstartEditorOnLoad.Start;
		}

		static void Start()
		{
			EditorApplication.update -= DatabaseQuickstartEditorOnLoad.Start;
            if (EditorPrefs.GetBool(KEY_PREFERENCES_STARTUP, true))
            {
                EditorPrefs.SetBool(KEY_PREFERENCES_STARTUP, false);

				PreferencesWindow.SetSidebarIndex(0);
				PreferencesWindow.OpenWindow();
            }
        }
    }
}