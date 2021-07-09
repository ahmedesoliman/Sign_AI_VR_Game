namespace GameCreator.Core
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using UnityEngine;
	using UnityEditor;
	using UnityEditorInternal;
	using GameCreator.DataStructures;
    using GameCreator.Variables;

	public class SelectTypePanel : PopupWindowContent
	{
		private const string SEARCHBOX_NAME = "searchbox";
        private const string SEARCHBOX_KEY = "gamecreator-selecttypepanel-searchbox-{0}";

        private static readonly char[] SEPARATOR = new char[] { '/' };

        private const BindingFlags BINDING_FLAGS = (
            BindingFlags.Public |
            BindingFlags.Static |
            BindingFlags.FlattenHierarchy
        );

        public const string ICONS_ACTIONS_PATH = "Assets/Plugins/GameCreator/Extra/Icons/Actions/";
        public const string ICONS_CONDITIONS_PATH = "Assets/Plugins/GameCreator/Extra/Icons/Conditions/";
        public const string ICONS_IGNITERS_PATH = "Assets/Plugins/GameCreator/Extra/Icons/Igniters/";
        public const string ICONS_VARIABLES_PATH = "Assets/Plugins/GameCreator/Extra/Icons/Variables/";
        public const string CUSTOM_ICON_PATH_VARIABLE = "CUSTOM_ICON_PATH";

		private const float ARROW_SIZE   = 13f;
		public const float WINDOW_WIDTH  = 200f;
		public const float WINDOW_HEIGHT = 300f;

		private class NodeData
		{
			public GUIContent content;
			public Type component;
			public int listIndex;

			public NodeData(string content, Type component = null)
			{
				Texture2D icon;
                string[] sections = content.Split(SEPARATOR);
                string name = sections[sections.Length - 1];

				if (component == null) icon = EditorGUIUtility.FindTexture("Folder Icon");
				else
				{
					string iconsPath = "";
					if (component.IsSubclassOf(typeof(ICondition))) iconsPath = ICONS_CONDITIONS_PATH;
					else if (component.IsSubclassOf(typeof(IAction))) iconsPath = ICONS_ACTIONS_PATH;
					else if (component.IsSubclassOf(typeof(Igniter))) iconsPath = ICONS_IGNITERS_PATH;
                    else if (component.IsSubclassOf(typeof(VariableBase))) iconsPath = ICONS_VARIABLES_PATH;

                    FieldInfo customIconsFieldInfo = component.GetField(CUSTOM_ICON_PATH_VARIABLE, BINDING_FLAGS);
					if (customIconsFieldInfo != null)
					{
						string customIconsPath = (string)customIconsFieldInfo.GetValue(null);
						if (!string.IsNullOrEmpty(customIconsPath))
						{
							iconsPath = customIconsPath;
						}
					}

                    icon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconsPath + name + ".png");
					if (icon == null) icon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconsPath + "Default.png");
					if (icon == null) icon = EditorGUIUtility.FindTexture("GameObject Icon");
				}

                this.content = new GUIContent(" " + name, icon);
				this.component = component;
				this.listIndex = 0;
			}
		}

		// PROPERTIES: ----------------------------------------------------------------------------

		private GUIStyle searchBoxStyle;
		private GUIStyle searchFieldStyle;
		private GUIStyle searchCloseOnStyle;
		private GUIStyle searchCloseOffStyle;
		private GUIStyle headerBoxStyle;
		private GUIStyle headerTitleStyle;
		private GUIStyle headerBackStyle;
		private GUIStyle elementSelectorStyle;

		private int listIndex = 0;
		private Rect listSelectedRect = Rect.zero;

		private string search = "";
		private bool searchFocus = true;
		private TreeNode<NodeData> categorizedTree;
		private List<TreeNode<NodeData>> uncategorizedList;

		private TreeNode<NodeData> currentBranch;
		private Stack<TreeNode<NodeData>> pathTrace;
		private KeyValuePair<string, TreeNode<NodeData>> cachedSearch;

		private Vector2 scroll;
		private Action<Type> callback;

		private string rootName;
		private Type baseType;
        private float winWidth;

		private bool keyPressedAny   = false;
		private bool keyPressedRight = false;
		private bool keyPressedBack  = false;
		private bool keyPressedUp    = false;
		private bool keyPressedDown  = false;
		private bool keyPressedEnter = false;
		private bool keyFlagVerticalMoved = false;

		// INITIALIZE METHODS: --------------------------------------------------------------------

        public SelectTypePanel(Action<Type> callback, string rootName, Type baseType, float width = 0.0f) : base()
		{
			this.callback = callback;
			this.rootName = rootName;
			this.baseType = baseType;
            this.winWidth = width;
		}

		public override void OnOpen()
		{
			this.searchBoxStyle = new GUIStyle(GUI.skin.FindStyle("TabWindowBackground"));
			this.searchFieldStyle = new GUIStyle(GUI.skin.FindStyle("SearchTextField"));
			this.searchCloseOnStyle = new GUIStyle(GUI.skin.FindStyle("SearchCancelButton"));
			this.searchCloseOffStyle = new GUIStyle(GUI.skin.FindStyle("SearchCancelButtonEmpty"));

			this.headerTitleStyle = new GUIStyle(GUI.skin.FindStyle("BoldLabel"));
			this.headerTitleStyle.padding = new RectOffset(
				20, this.headerTitleStyle.padding.right,
				this.headerTitleStyle.padding.top, this.headerTitleStyle.padding.bottom
			);

			this.headerBoxStyle = new GUIStyle(GUI.skin.FindStyle("IN BigTitle"));
			this.headerBoxStyle.margin = new RectOffset(0,0,0,0);

			this.elementSelectorStyle = new GUIStyle(GUI.skin.FindStyle("MenuItem"));
			this.elementSelectorStyle.fixedHeight = 20f;
			this.elementSelectorStyle.padding = new RectOffset(
				5,5,this.elementSelectorStyle.padding.top, this.elementSelectorStyle.padding.bottom
			);
			this.elementSelectorStyle.imagePosition = ImagePosition.ImageLeft;

			this.scroll = Vector2.zero;
			this.searchFocus = true;
			this.cachedSearch = new KeyValuePair<string, TreeNode<NodeData>>();

			this.CreateList();
			this.editorWindow.Focus();
            this.search = EditorPrefs.GetString(string.Format(SEARCHBOX_KEY, this.rootName), "");
		}

		public override void OnClose ()
		{
            EditorPrefs.SetString(string.Format(SEARCHBOX_KEY, this.rootName), this.search);
			base.OnClose ();
		}

		public override Vector2 GetWindowSize()
		{
			return new Vector2(
                Mathf.Max(winWidth, WINDOW_WIDTH), 
                WINDOW_HEIGHT
            );
		}

		private void CreateList()
		{
			NodeData rootData = new NodeData(this.rootName);
			this.categorizedTree = new TreeNode<NodeData>(this.rootName, rootData);
			this.uncategorizedList = new List<TreeNode<NodeData>>(
                new TreeNode<NodeData>(this.rootName, rootData)
            );

			List<Type> types = this.GetAllClassTypesOf(this.baseType);
			int typesSize = types.Count;

			for (int i = 0; i < typesSize; ++i)
			{
                string actionName = (string)types[i].GetField("NAME", BINDING_FLAGS).GetValue(null);
				string[] categories = SelectTypePanel.GetCategories(actionName);

				TreeNode<NodeData> node = this.categorizedTree;
				for (int j = 0; j < categories.Length; ++j)
				{
					if (node.HasChild(categories[j]))
					{
						node = node.GetChild(categories[j]);
					}
					else
					{
						NodeData nodeData = new NodeData(categories[j]);
						TreeNode<NodeData> treeNode = new TreeNode<NodeData>(categories[j], nodeData);
						node = node.AddChild(treeNode);
					}
				}

                NodeData leafData = new NodeData(actionName, types[i]);
                node.AddChild(new TreeNode<NodeData>(actionName, leafData));
                this.uncategorizedList.Add(new TreeNode<NodeData>(actionName, leafData));
			}

			this.currentBranch = this.categorizedTree;
			this.pathTrace = new Stack<TreeNode<NodeData>>();
		}

		// WINDOW GUI: ----------------------------------------------------------------------------

		public override void OnGUI(Rect rect)
		{
			this.HandleKeyboardInput();
			this.PaintSearch();

			if (string.IsNullOrEmpty(this.search))
			{
				this.listIndex = this.currentBranch.GetData().listIndex;
				this.PaintElements(this.currentBranch, (this.pathTrace.Count > 0));
			}
			else
			{
				if (this.cachedSearch.Key != this.search)
				{
					TreeNode<NodeData> emptyTree = new TreeNode<NodeData>("Search", null);
					this.cachedSearch = new KeyValuePair<string, TreeNode<NodeData>>(this.search, emptyTree);
					string search = this.search.ToLower();
					int listCount = this.uncategorizedList.Count;

					for (int i = 0; i < listCount; ++i)
					{
						string childID = this.uncategorizedList[i].GetID().ToLower();
						if (childID.Contains(search))
						{
							this.cachedSearch.Value.AddChild(this.uncategorizedList[i]);
						}
					}

					this.listIndex = 0;
				}

				this.PaintElements(this.cachedSearch.Value, false);
			}

			bool repaintEvent = false;
			repaintEvent = repaintEvent || UnityEngine.Event.current.type == EventType.MouseMove;
			repaintEvent = repaintEvent || UnityEngine.Event.current.type == EventType.MouseDown;
			repaintEvent = repaintEvent || this.keyPressedAny;

			if (repaintEvent) this.editorWindow.Repaint();
		}

		private void HandleKeyboardInput()
		{
			this.keyPressedAny   = false;
			this.keyPressedRight = false;
			this.keyPressedBack  = false;
			this.keyPressedUp    = false;
			this.keyPressedDown  = false;
			this.keyPressedEnter = false;

			if (UnityEngine.Event.current.type != EventType.KeyDown) return;

			this.keyPressedAny   = true;
			this.keyPressedRight = (UnityEngine.Event.current.keyCode == KeyCode.RightArrow);
			this.keyPressedBack  = (UnityEngine.Event.current.keyCode == KeyCode.LeftArrow);
			this.keyPressedUp    = (UnityEngine.Event.current.keyCode == KeyCode.UpArrow);
			this.keyPressedDown  = (UnityEngine.Event.current.keyCode == KeyCode.DownArrow);

			this.keyPressedEnter = (
				UnityEngine.Event.current.keyCode == KeyCode.KeypadEnter ||
				UnityEngine.Event.current.keyCode == KeyCode.Return
			);

			this.keyFlagVerticalMoved = (
				this.keyPressedUp ||
				this.keyPressedDown
			);
		}

		private void PaintSearch()
		{
			EditorGUILayout.BeginHorizontal(this.searchBoxStyle);

			GUI.SetNextControlName(SEARCHBOX_NAME);
			this.search = EditorGUILayout.TextField(this.search, this.searchFieldStyle);

			if (this.searchFocus)
			{
				EditorGUI.FocusTextInControl(SEARCHBOX_NAME);
				this.searchFocus = false;
			}

			GUIStyle style = (string.IsNullOrEmpty(this.search) ? this.searchCloseOffStyle : this.searchCloseOnStyle);
			if (GUILayout.Button("", style))
			{
				this.search = "";
				GUIUtility.keyboardControl = 0;
				this.searchFocus = true;
			}

			EditorGUILayout.EndHorizontal();
		}

		private bool PaintTitle(string title, bool backButton)
		{
			bool goBack = false;
			EditorGUILayout.BeginHorizontal(this.headerBoxStyle);

			if (backButton && this.keyPressedBack)
			{
				UnityEngine.Event.current.Use();
			}

			if (backButton && (GUILayout.Button(title, this.headerTitleStyle) || this.keyPressedBack))
			{
				this.listIndex = 0;
				goBack = true;
			}
			else if (!backButton)
			{
				EditorGUILayout.LabelField(title, this.headerTitleStyle);
			}

			if (UnityEngine.Event.current.type == EventType.Repaint && backButton)
			{
				Rect buttonRect = GUILayoutUtility.GetLastRect();
				CoreGUIStyles.GetButtonLeftArrow().Draw(
					new Rect(
						buttonRect.x,
						buttonRect.y + buttonRect.height/2.0f - ARROW_SIZE/2.0f,
						ARROW_SIZE, ARROW_SIZE
					),
					false, false, false, false
				);
			}

			EditorGUILayout.EndHorizontal();
			return goBack;
		}

		private void PaintElements(TreeNode<NodeData> branch, bool backButton)
		{
			if (this.PaintTitle(branch.GetID(), backButton))
			{
				this.currentBranch = this.pathTrace.Pop();
				this.listIndex = this.currentBranch.GetData().listIndex;
			}

			this.scroll = EditorGUILayout.BeginScrollView(this.scroll, GUIStyle.none, GUI.skin.verticalScrollbar);
			int elemIndex = 0;

			foreach (TreeNode<NodeData> element in branch)
			{
				NodeData nodeData = element.GetData();

				bool mouseEnter = this.listIndex == elemIndex && UnityEngine.Event.current.type == EventType.MouseDown;

				Rect buttonRect = GUILayoutUtility.GetRect(nodeData.content, this.elementSelectorStyle);

				bool buttonHasFocus = this.listIndex == elemIndex;
				if (UnityEngine.Event.current.type == EventType.Repaint)
				{
					if (this.listIndex == elemIndex) this.listSelectedRect = buttonRect;

					this.elementSelectorStyle.Draw(
						buttonRect,
						nodeData.content,
						buttonHasFocus,
						buttonHasFocus,
						false,
						false
					);

					if (nodeData.component == null)
					{
						CoreGUIStyles.GetButtonRightArrow().Draw(
							new Rect(
								buttonRect.x + buttonRect.width - ARROW_SIZE,
								buttonRect.y + buttonRect.height/2.0f - ARROW_SIZE/2.0f,
								ARROW_SIZE, ARROW_SIZE
							),
							false, false, false, false
						);
					}
				}

				if (buttonHasFocus)
				{
					if (nodeData.component == null)
					{
						if (mouseEnter || this.keyPressedRight || this.keyPressedEnter)
						{
							if (this.keyPressedRight) UnityEngine.Event.current.Use();
							if (this.keyPressedEnter) UnityEngine.Event.current.Use();

							this.currentBranch.GetData().listIndex = this.listIndex;
							this.pathTrace.Push(this.currentBranch);

							this.currentBranch = this.currentBranch.GetChild(element.GetID());
							this.listIndex = 0;
						}
					}
					else
					{
						if (mouseEnter || this.keyPressedEnter)
						{
							if (this.keyPressedEnter) UnityEngine.Event.current.Use();
							if (this.callback != null) this.callback(nodeData.component);
							this.editorWindow.Close();
						}
					}
				}

				if (UnityEngine.Event.current.type == EventType.MouseMove &&
					GUILayoutUtility.GetLastRect().Contains(UnityEngine.Event.current.mousePosition))
				{
					this.listIndex = elemIndex;
					this.currentBranch.GetData().listIndex = this.listIndex;
				}

				++elemIndex;
			}

			if (this.keyPressedDown && this.listIndex < elemIndex - 1)
			{
				this.listIndex++;
				this.currentBranch.GetData().listIndex = this.listIndex;
				UnityEngine.Event.current.Use();
			}
			else if (this.keyPressedUp && this.listIndex > 0)
			{
				this.listIndex--;
				this.currentBranch.GetData().listIndex = this.listIndex;
				UnityEngine.Event.current.Use();
			}

			EditorGUILayout.EndScrollView();
			float scrollHeight = GUILayoutUtility.GetLastRect().height;

			if (UnityEngine.Event.current.type == EventType.Repaint && this.keyFlagVerticalMoved)
			{
				this.keyFlagVerticalMoved = false;
				if (this.listSelectedRect != Rect.zero)
				{
					if (this.scroll.y > this.listSelectedRect.y)
					{
						this.scroll = Vector2.up * (this.listSelectedRect.position.y);
						this.editorWindow.Repaint();
					}
					else if (this.scroll.y + scrollHeight < this.listSelectedRect.position.y + this.listSelectedRect.size.y)
					{
						float positionY = this.listSelectedRect.y + this.listSelectedRect.height - scrollHeight;
						this.scroll = Vector2.up * positionY;
						this.editorWindow.Repaint();
					}
				}
			}
		}

		// PRIVATE METHODS: -----------------------------------------------------------------------

		private List<Type> GetAllClassTypesOf(Type parentType)
		{
			List<Type> result = new List<Type>();
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; ++i)
			{
                result.AddRange(assemblies[i].GetTypes().Where(
                    myType => myType.IsClass && 
                    !myType.IsAbstract && 
                    myType.IsSubclassOf(parentType)
                ));
			}

			return result;
		}

		public static string GetName(string name)
		{
			string[] categories = name.Split('/');
			if (categories.Length > 0) return categories[categories.Length - 1];
			return "no-name";
		}

		private static string[] GetCategories(string name)
		{
			string[] categories = name.Split('/');
			if (categories.Length > 1)
			{
				string[] subarrayCategories = new string[categories.Length - 1];
				for (int i = 0; i < subarrayCategories.Length; ++i)
				{
					subarrayCategories[i] = categories[i];
				}

				categories = subarrayCategories;
			}
			else
			{
				categories = new string[0];
			}

			return categories;
		}
	}
}
