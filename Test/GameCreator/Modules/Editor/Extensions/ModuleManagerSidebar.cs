namespace GameCreator.ModuleManager
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using GameCreator.Core;

    public abstract class ModuleManagerSidebar
    {
        private const string HEADER_PATH_L = "Assets/Plugins/GameCreator/Modules/Icons/UI/HeaderLight.png";
        private const string HEADER_PATH_D = "Assets/Plugins/GameCreator/Modules/Icons/UI/HeaderDark.png";

        private const int ICON_SIZE = 18;

        // PROPERTIES: ----------------------------------------------------------------------------

        private static Texture2D TEXTURE_HEADER;
        private static GUIStyle BUTTON_NORM;
        private static GUIStyle BUTTON_ACTV;

        private static GUIStyle BUTTON_LOADING;
        private static GUIStyle BUTTON_TRYAGAIN;

        // PAINT METHODS: -------------------------------------------------------------------------

        public static bool PaintButton(GUIContent content, bool isActive)
        {
            GUIStyle style = (isActive ? GetButtonStyleActive() : GetButtonStyleNormal());
            bool click = GUILayout.Button(content, style);

            if (content.image != null)
            {
                Rect rect = GUILayoutUtility.GetLastRect();
                Rect iconRect = new Rect(
                    rect.x + 5f,
                    rect.y + (rect.height / 2.0f - ICON_SIZE / 2.0f),
                    ICON_SIZE,
                    ICON_SIZE
                );

                GUI.DrawTexture(iconRect, content.image);
            }

            return click;
        }

        public static void PaintHeader()
        {
            EditorGUILayout.Space();
            Texture2D texture = GetTextureHeader();
            Rect headerRect = GUILayoutUtility.GetRect(ModuleManagerWindow.WINDOW_SIDE_WIDTH, 25f);

            if (UnityEngine.Event.current.type == EventType.Repaint)
            {
                GUI.DrawTexture(headerRect, texture);
            }

            EditorGUILayout.Space();
        }

        public static void PaintSidebarProjects()
        {
            ModuleManifest[] manifests = ModuleManager.GetProjectManifests();
            for (int i = 0; i < manifests.Length; ++i)
            {
                GUIContent text = new GUIContent(
                    manifests[i].module.displayName,
                    ModuleManager.GetModuleIcon(manifests[i].module.moduleID)
                );

                if (ModuleManagerSidebar.PaintButton(text, ModuleManagerWindow.WINDOW.sidebarIndex == i))
                {
                    ModuleManagerWindow.WINDOW.sidebarIndex = i;
                    ModuleManagerWindow.WINDOW.Repaint();
                }
            }
        }

        // STYLE METHODS: -------------------------------------------------------------------------

        private static GUIStyle GetButtonStyleNormal()
        {
            if (BUTTON_NORM == null)
            {
                BUTTON_NORM = new GUIStyle(GUI.skin.GetStyle("MenuItem"));
                BUTTON_NORM.alignment = TextAnchor.MiddleLeft;
                BUTTON_NORM.padding.left = ICON_SIZE + 10;
                BUTTON_NORM.fixedHeight = 25f;
                BUTTON_NORM.hover = BUTTON_NORM.normal;
            }

            return BUTTON_NORM;
        }

        private static GUIStyle GetButtonStyleActive()
        {
            if (BUTTON_ACTV == null)
            {
                BUTTON_ACTV = new GUIStyle(GUI.skin.GetStyle("MenuItem"));
                BUTTON_ACTV.alignment = TextAnchor.MiddleLeft;
                BUTTON_ACTV.padding.left = ICON_SIZE + 10;
                BUTTON_ACTV.fixedHeight = 25f;
                BUTTON_ACTV.normal = BUTTON_ACTV.hover;
            }

            return BUTTON_ACTV;
        }

        private static Texture2D GetTextureHeader()
        {
            if (TEXTURE_HEADER == null)
            {
                string path = (EditorGUIUtility.isProSkin ? HEADER_PATH_D : HEADER_PATH_L);
                TEXTURE_HEADER = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (TEXTURE_HEADER == null) TEXTURE_HEADER = Texture2D.blackTexture;
            }

            return TEXTURE_HEADER;
        }

        private static GUIStyle GetLoadingButtonStyle()
        {
            if (BUTTON_LOADING == null)
            {
                BUTTON_LOADING = new GUIStyle(GetTryAgainButtonStyle());
                BUTTON_LOADING.alignment = TextAnchor.MiddleCenter;
                BUTTON_LOADING.normal = BUTTON_LOADING.onNormal;
                BUTTON_LOADING.hover = BUTTON_LOADING.onHover;
                BUTTON_LOADING.active = BUTTON_LOADING.onActive;
                BUTTON_LOADING.focused = BUTTON_LOADING.onFocused;
                BUTTON_LOADING.richText = true;
            }

            return BUTTON_LOADING;
        }

        private static GUIStyle GetTryAgainButtonStyle()
        {
            if (BUTTON_TRYAGAIN == null)
            {
                BUTTON_TRYAGAIN = new GUIStyle(GUI.skin.GetStyle("LargeButton"));
                BUTTON_TRYAGAIN.alignment = TextAnchor.MiddleCenter;
                BUTTON_TRYAGAIN.richText = true;
            }
            return BUTTON_TRYAGAIN;
        }
    }
}