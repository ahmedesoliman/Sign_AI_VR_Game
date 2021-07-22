namespace GameCreator.ModuleManager
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEditor;
    using UnityEditor.AnimatedValues;
    using GameCreator.Core;

    public abstract class ModuleManagerContent
    {
        private const string PANEL_PATH = "Assets/Plugins/GameCreator/Modules/Icons/UI/PanelBackground.png";
        private const string DOT_G_PATH = "Assets/Plugins/GameCreator/Modules/Icons/UI/DotGreen.png";
        private const string DOT_O_PATH = "Assets/Plugins/GameCreator/Modules/Icons/UI/DotOrange.png";
        private const string DOT_R_PATH = "Assets/Plugins/GameCreator/Modules/Icons/UI/DotRed.png";

        // PROPERTIES: ----------------------------------------------------------------------------

        private static Texture2D DOT_G;
        private static Texture2D DOT_O;
        private static Texture2D DOT_R;

        private static GUIStyle LARGE_BUTTON;
        private static GUIStyle TITLE_LABEL;
        private static GUIStyle PANEL_UPDATE;
        private static GUIStyle LARGE_BUTTON_L;
        private static GUIStyle LARGE_BUTTON_M;
        private static GUIStyle LARGE_BUTTON_R;
        private static GUIStyle TEXT;

        private static AnimBool ANIMBOOL_UPDATE;
        private static string CURRENT_MODULE_ID = "";

        // PAINT METHODS: -------------------------------------------------------------------------

        public static void PaintProjectModule(ModuleManifest manifest)
        {
            if (CURRENT_MODULE_ID != manifest.module.moduleID)
            {
                ModuleManagerContent.InitializeModule(manifest);
                CURRENT_MODULE_ID = manifest.module.moduleID;
            }

            bool isEnabled = ModuleManager.IsEnabled(manifest.module);
            bool updateAvail = ModuleManager.IsUpdateAvailable(manifest);
            bool assetModuleExists = ModuleManager.AssetModuleExists(manifest.module);
            AssetModule assetModule = ModuleManager.GetAssetModule(manifest.module.moduleID);

            ModuleManagerContent.PaintTitle(manifest.module);

            EditorGUILayout.BeginHorizontal(ModuleManagerContent.GetPanelStyle());
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Backup", ModuleManagerContent.GetLargeButtonStyle()))
            {
                ModuleManager.Backup(manifest);
            }

            EditorGUILayout.Space();
            EditorGUI.BeginDisabledGroup(!isEnabled || !updateAvail);
            if (GUILayout.Button("Update", ModuleManagerContent.GetLargeButtonLeft()))
            {
                ModuleManager.Update(manifest);
            }
            EditorGUI.EndDisabledGroup();

            if (!isEnabled && GUILayout.Button("Enable", ModuleManagerContent.GetLargeButtonMid()))
            {
                ModuleManager.Enable(manifest);
            }

            if (isEnabled && GUILayout.Button("Disable", ModuleManagerContent.GetLargeButtonMid()))
            {
                ModuleManager.Disable(manifest);
            }

            EditorGUI.BeginDisabledGroup(!assetModuleExists || isEnabled);
            if (GUILayout.Button("Remove", ModuleManagerContent.GetLargeButtonRight()))
            {
                ModuleManager.Remove(manifest);
            }
            EditorGUI.EndDisabledGroup();

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            ModuleManagerContent.PaintModule(manifest.module);

            EditorGUILayout.Space();
            ModuleManagerContent.PaintDependencies(
                "Dependencies ({0})",
                manifest.module.dependencies
            );

            if (updateAvail && assetModule != null)
            {
                EditorGUILayout.Space();
                ModuleManagerContent.PaintDependencies(
                    "Update Dependencies ({0})", 
                    assetModule.module.dependencies
                );
            }
        }

        private static void PaintTitle(Module module)
        {
            EditorGUILayout.BeginHorizontal(GUILayout.Height(35));

            GUIContent content = new GUIContent(
                " " + module.displayName,
                ModuleManager.GetModuleIcon(module.moduleID)
            );

            EditorGUILayout.LabelField(content, ModuleManagerContent.GetTitleLabelStyle());

            EditorGUILayout.EndHorizontal();
        }

        public static void PaintContentMessage(string message = "No module selected")
        {
            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            EditorGUILayout.LabelField(message, EditorStyles.centeredGreyMiniLabel);

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private static void InitializeModule(ModuleManifest manifest)
        {
            
            ANIMBOOL_UPDATE = new AnimBool(false, ModuleManagerContent.RepaintModuleManager);
            ANIMBOOL_UPDATE.speed = 3.0f;
        }

        private static void RepaintModuleManager()
        {
            if (ModuleManagerWindow.WINDOW == null) return;
            ModuleManagerWindow.WINDOW.Repaint();
        }

        private static void PaintModule(Module module)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Module ID", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(module.moduleID);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Version", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(module.version.ToString());

            if (!string.IsNullOrEmpty(module.authorName) && !string.IsNullOrEmpty(module.authorMail))
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Author", EditorStyles.boldLabel);
                EditorGUILayout.LabelField(module.authorName);
                EditorGUILayout.LabelField(module.authorMail);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Description", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(module.description, ModuleManagerContent.GetTextStyle());

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Tags", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < module.tags.Length; ++i)
            {
                GUILayout.Button(module.tags[i], EditorStyles.helpBox);
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private static void PaintDependencies(string title, Dependency[] dependencies)
        {
            EditorGUILayout.LabelField(
                string.Format(title, dependencies.Length),
                EditorStyles.boldLabel
            );

            if (dependencies.Length > 0)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                for (int i = 0; i < dependencies.Length; ++i)
                {
                    ModuleManifest depManifest = ModuleManager.GetModuleManifest(
                        dependencies[i].moduleID
                    );

                    Texture2D dot = ModuleManagerContent.GetDotG();
                    if (depManifest == null)
                    {
                        dot = ModuleManagerContent.GetDotR();
                    }
                    else if (depManifest.module.version.Higher(dependencies[i].version) || 
                             !ModuleManager.IsEnabled(depManifest.module))
                    {
                        dot = ModuleManagerContent.GetDotO();
                    }

                    string depName = string.Format(
                        " {0} - {1}",
                        dependencies[i].moduleID,
                        dependencies[i].version
                    );

                    GUIContent depContent = new GUIContent(depName, dot);
                    EditorGUILayout.LabelField(depContent);

                    Rect depRect = GUILayoutUtility.GetLastRect();
                    EditorGUIUtility.AddCursorRect(depRect, MouseCursor.Link);
                    if (UnityEngine.Event.current.type == EventType.MouseUp &&
                        depRect.Contains(UnityEngine.Event.current.mousePosition))
                    {
                        Application.OpenURL("https://hub.gamecreator.io/content/modules");
                    }
                }
                EditorGUILayout.EndVertical();
            }
        }

        // STYLE METHODS: -------------------------------------------------------------------------

        private static GUIStyle GetLargeButtonStyle()
        {
            if (LARGE_BUTTON == null)
            {
                LARGE_BUTTON = new GUIStyle(GUI.skin.GetStyle("Button"));
                LARGE_BUTTON.padding = new RectOffset(20, 20, 0, 0);
                LARGE_BUTTON.fixedHeight = 30;
            }

            return LARGE_BUTTON;
        }

        private static GUIStyle GetTitleLabelStyle()
        {
            if (TITLE_LABEL == null)
            {
                TITLE_LABEL = new GUIStyle(EditorStyles.label);
                TITLE_LABEL.fixedHeight = 30;
                TITLE_LABEL.alignment = TextAnchor.MiddleLeft;
                TITLE_LABEL.fontSize = 14;
            }

            return TITLE_LABEL;
        }

        private static GUIStyle GetPanelStyle()
        {
            if (PANEL_UPDATE == null)
            {
                PANEL_UPDATE = new GUIStyle();
                PANEL_UPDATE.normal.background = AssetDatabase.LoadAssetAtPath<Texture2D>(PANEL_PATH);
                PANEL_UPDATE.padding = new RectOffset(10, 10, 20, 20);
                PANEL_UPDATE.alignment = TextAnchor.MiddleCenter;
            }

            return PANEL_UPDATE;
        }

        public static GUIStyle GetLargeButtonLeft()
        {
            if (LARGE_BUTTON_L == null)
            {
                LARGE_BUTTON_L = new GUIStyle(GUI.skin.GetStyle("ButtonLeft"));
                LARGE_BUTTON_L.padding = new RectOffset(20, 20, 0, 0);
                LARGE_BUTTON_L.fixedHeight = 30;
                LARGE_BUTTON_L.alignment = TextAnchor.MiddleCenter;
                LARGE_BUTTON_L.richText = true;
            }

            return LARGE_BUTTON_L;
        }

        public static GUIStyle GetLargeButtonRight()
        {
            if (LARGE_BUTTON_R == null)
            {
                LARGE_BUTTON_R = new GUIStyle(GUI.skin.GetStyle("ButtonRight"));
                LARGE_BUTTON_R.padding = new RectOffset(20, 20, 0, 0);
                LARGE_BUTTON_R.fixedHeight = 30;
                LARGE_BUTTON_R.alignment = TextAnchor.MiddleCenter;
                LARGE_BUTTON_R.richText = true;
            }

            return LARGE_BUTTON_R;
        }

        public static GUIStyle GetLargeButtonMid()
        {
            if (LARGE_BUTTON_M == null)
            {
                LARGE_BUTTON_M = new GUIStyle(GUI.skin.GetStyle("ButtonMid"));
                LARGE_BUTTON_M.padding = new RectOffset(20, 20, 0, 0);
                LARGE_BUTTON_M.fixedHeight = 30;
                LARGE_BUTTON_M.alignment = TextAnchor.MiddleCenter;
                LARGE_BUTTON_M.richText = true;
            }

            return LARGE_BUTTON_M;
        }

        private static GUIStyle GetTextStyle()
        {
            if (TEXT == null)
            {
                TEXT = new GUIStyle(GUI.skin.GetStyle("Label"));
                TEXT.wordWrap = true;
                TEXT.richText = true;
            }

            return TEXT;
        }

        private static Texture2D GetDotG()
        {
            if (DOT_G == null) DOT_G = AssetDatabase.LoadAssetAtPath<Texture2D>(DOT_G_PATH);
            return DOT_G;
        }

        private static Texture2D GetDotO()
        {
            if (DOT_O == null) DOT_O = AssetDatabase.LoadAssetAtPath<Texture2D>(DOT_O_PATH);
            return DOT_O;
        }

        private static Texture2D GetDotR()
        {
            if (DOT_R == null) DOT_R = AssetDatabase.LoadAssetAtPath<Texture2D>(DOT_R_PATH);
            return DOT_R;
        }
    }
}