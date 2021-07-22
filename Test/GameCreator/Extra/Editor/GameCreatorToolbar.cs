namespace GameCreator.Core
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Events;
    using GameCreator.Characters;
    using GameCreator.Camera;
    using GameCreator.Variables;

    public static class GameCreatorToolbar
    {
        private enum BTN_POS { L, M, R };

        public class Item
        {
            public string iconPath;
            public string tooltip;
            public UnityAction callback;
            public int priority;
            private GUIContent content;
            private Texture2D texture;

            public Item(string iconPath, string tooltip, UnityAction callback, int priority = 100)
            {
                this.iconPath = iconPath;
                this.tooltip  = tooltip;
                this.callback = callback;
                this.priority = priority;
            }

            public GUIContent GetContent()
            {
                if (this.content == null)
                {
                    this.content = new GUIContent("", this.tooltip);
                }

                return this.content;
            }

            public Texture2D GetTexture()
            {
                if (this.texture == null)
                {
                    this.texture = AssetDatabase.LoadAssetAtPath<Texture2D>(this.iconPath);
                }

                return this.texture;
            }
        }

        public const string PATH_ICONS = "Assets/Plugins/GameCreator/Extra/Icons/Toolbar/{0}";

        private const string KEY_TOOLBAR_ENABLED = "gamecreator-toolbar-enabled";
        private const bool TOOLBAR_DEFAULT = true;

        private const float TOOLBAR_HEIGHT = 20f;
        private const float BUTTONS_WIDTH  = 20f;

        private static bool DRAGGING = false;
        private static Vector2 MOUSE_POSITION = Vector2.zero;

        private static float MOVE_OFFSET_X = 0.0f;
        private static float MOVE_OFFSET_Y = 0.0f;

        private static bool STYLES_INITIALIZED = false;
        private static GUIStyle BTN_LFT;
        private static GUIStyle BTN_MID;
        private static GUIStyle BTN_RHT;

        private static List<Item> ITEMS = new List<Item>();
        public static Stack<Item> REGISTER_ITEMS = new Stack<Item>();

        // PUBLIC METHDOS: ------------------------------------------------------------------------

        [InitializeOnLoadMethod]
        public static void Initialize()
        {
            if (IsEnabled()) Enable();
            else Disable();

            RegisterDefaultItems();
        }

        [MenuItem("Game Creator/Tools/Show Toolbar %&t", true)]
        public static bool ShowToolbarValidate()
        {
            return !IsEnabled();
        }

        [MenuItem("Game Creator/Tools/Show Toolbar %&t", false)]
        public static void ShowToolbar()
        {
            Enable();
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private static void Enable()
        {
            EditorPrefs.SetBool(KEY_TOOLBAR_ENABLED, true);

            #if UNITY_2019_2_OR_NEWER
            SceneView.duringSceneGui += OnPaintToolbar;
            #else
            SceneView.onSceneGUIDelegate += OnPaintToolbar;
            #endif

            DatabaseGeneral general = DatabaseGeneral.Load();
            MOVE_OFFSET_X = general.toolbarPositionX;
            MOVE_OFFSET_Y = general.toolbarPositionY;

            SceneView.RepaintAll();
        }

        private static void Disable()
        {
            EditorPrefs.SetBool(KEY_TOOLBAR_ENABLED, false);

            #if UNITY_2019_2_OR_NEWER
            SceneView.duringSceneGui -= OnPaintToolbar;
            #else
            SceneView.onSceneGUIDelegate -= OnPaintToolbar;
            #endif

            SceneView.RepaintAll();
        }

        private static bool IsEnabled()
        {
            return EditorPrefs.GetBool(KEY_TOOLBAR_ENABLED, TOOLBAR_DEFAULT);
        }

        private static void RegisterDefaultItems()
        {
            RegisterItem("trigger.png", "Create Trigger", TriggerEditor.CreateTrigger, 2);
            RegisterItem("conditions.png", "Create Conditions", ConditionsEditor.CreateConditions, 3);
            RegisterItem("actions.png", "Create Actions", ActionsEditor.CreateAction, 4);
            RegisterItem("character.png", "Create Character", CharacterEditor.CreateCharacter, 5);
            RegisterItem("player.png", "Create Player", PlayerCharacterEditor.CreatePlayer, 6);
            RegisterItem("marker.png", "Create Navigation Marker", NavigationMarkerEditor.CreateMarker, 7);
            RegisterItem("hotspot.png", "Create Hotspot", HotspotEditor.CreateHotspot, 8);
            RegisterItem("motor.png", "Create Camera Motor", CameraMotorEditor.CreateCameraMotor, 9);
            RegisterItem("localvariables.png", "Create Local Variables", LocalVariablesEditor.CreateLocalVariables, 10);
            RegisterItem("listvariables.png", "Create List Variables", ListVariablesEditor.CreateListVariables, 11);

            RegisterItem("close.png", "Close the Toolbar", GameCreatorToolbar.Disable, 998);
            RegisterItem("drag.png", "Move the Toolbar", null, 999);
        }

        private static void RegisterItem(string icon, string hint, UnityAction callback, int priority)
        {
            REGISTER_ITEMS.Push(new Item(string.Format(PATH_ICONS, icon), hint, callback, priority));
        }

        // PAINT METHODS: -------------------------------------------------------------------------

        private static void OnPaintToolbar(SceneView sceneview)
        {
            GUISkin prevSkin = GUI.skin;
            GUI.skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene);

            bool registeredItem = false;
            while (REGISTER_ITEMS.Count > 0)
            {
                registeredItem = true;
                Item item = REGISTER_ITEMS.Pop();
                ITEMS.Add(item);
            }

            if (registeredItem) ITEMS.Sort((Item x, Item y) => x.priority.CompareTo(y.priority));
            if (ITEMS.Count == 0) return;

            Rect rect = new Rect(
                MOVE_OFFSET_X, 
                MOVE_OFFSET_Y, 
                BUTTONS_WIDTH * ITEMS.Count,
                TOOLBAR_HEIGHT
            );

            bool mouseInRect = rect.Contains(UnityEngine.Event.current.mousePosition);
            if (UnityEngine.Event.current.type == EventType.MouseUp) DRAGGING = false;
            if (UnityEngine.Event.current.type == EventType.MouseDown && mouseInRect)
            {
                MOUSE_POSITION = UnityEngine.Event.current.mousePosition;
                DRAGGING = true;
            }

            if (DRAGGING)
            {
                Vector2 delta = UnityEngine.Event.current.mousePosition - MOUSE_POSITION;

                MOVE_OFFSET_X += delta.x;
                MOVE_OFFSET_Y += delta.y;
                SceneView.currentDrawingSceneView.Repaint();
            }

            MOUSE_POSITION = UnityEngine.Event.current.mousePosition;

            Handles.BeginGUI();
            GUILayout.BeginArea(rect);
            EditorGUILayout.BeginHorizontal();

            for (int i = 0; i < ITEMS.Count - 1; ++i)
            {
                BTN_POS position = (i == 0 ? BTN_POS.L : BTN_POS.M);
                PaintButton(ITEMS[i], position);
            }

            int panIndex = ITEMS.Count - 1;
            PaintButton(ITEMS[panIndex], BTN_POS.R, MouseCursor.Pan);

            EditorGUILayout.EndHorizontal();
            GUILayout.EndArea();
            Handles.EndGUI();

            GUI.skin = prevSkin;
        }

        private static void PaintButton(Item item, BTN_POS position, MouseCursor cursor = MouseCursor.Link)
        {
            if (!STYLES_INITIALIZED)
            {
                BTN_LFT = new GUIStyle(GUI.skin.GetStyle("ButtonLeft"));
                BTN_MID = new GUIStyle(GUI.skin.GetStyle("ButtonMid"));
                BTN_RHT = new GUIStyle(GUI.skin.GetStyle("ButtonRight"));
                BTN_RHT.normal = BTN_RHT.active;
                BTN_RHT.onNormal = BTN_RHT.onActive;

                STYLES_INITIALIZED = true;
            }

            GUIStyle style;
            switch (position)
            {
                case BTN_POS.L: style = BTN_LFT; break;
                case BTN_POS.M: style = BTN_MID; break;
                case BTN_POS.R: style = BTN_RHT; break;
                default: style = null; break;
            }

            if (GUILayout.Button(item.GetContent(), style, GUILayout.Width(BUTTONS_WIDTH)))
            {
                if (item.callback != null) item.callback.Invoke();
            }

            Rect rect = GUILayoutUtility.GetLastRect();
            Rect textureRect = new Rect(
                rect.x + (rect.width/2.0f - rect.height/2.0f),
                rect.y,
                rect.height,
                rect.height
            );

            GUI.DrawTexture(textureRect, item.GetTexture());
            EditorGUIUtility.AddCursorRect(rect, cursor);
        }
    }
}